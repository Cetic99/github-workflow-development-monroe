/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { useTranslation } from '@domain/administration/stores'
import { useMessages } from '@domain/administration/queries'
import { useRef, useState } from 'react'
import styled from '@emotion/styled'
import { useSaveMessage } from '@domain/administration/commands'
import Button from '@ui/components/button'
import Table from '@ui/components/table'
import Modal from '@ui/components/modal'
import Pagination from '@ui/components/pagination'
import TextInput from '@ui/components/inputs/text-input'
import Divider from '@ui/components/divider'
import FullPageLoader from '@ui/components/full-page-loader'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import IconEdit from '@ui/components/icons/IconEdit'
import FilterButton from '@ui/components/filter-button'
import ItemsPerPage from '@ui/components/items-per-page'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  height: inherit;

  & .header {
    padding: 1rem 0;
    align-items: end;
    display: flex;
    flex-direction: column;
    margin-right: 0.75rem;
  }

  & .table-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .table-footer {
      display: flex;
      gap: 2rem;
      justify-content: center;
      margin-bottom: 2rem;
      align-items: flex-end;
      padding-top: 1.5rem;

      & .items-per-page {
        > div {
          flex-direction: column;
          align-items: center;
        }
      }
      & .pagination {
        display: flex;
        align-self: end;
      }

      @media screen and (max-width: 800px) {
        flex-direction: column-reverse;
        align-items: center;
        gap: 0.5rem;
      }
    }

    & .no-data-label {
      text-align: center;
      padding: 2rem;
      color: var(--text-inactive-dark);
      font-weight: 600;
      font-size: 1.75rem;
      line-height: 1.875rem;
      font-family: Poppins;
    }
  }

  & .pts-footer {
    margin-top: 2rem 0;
    display: flex;
    gap: 4rem;
    width: 100%;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;

    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }

  & .modal-content {
    padding-top: 2rem;
    display: flex;
    flex-direction: column;
    gap: 0.875rem;

    & .dropdown {
      width: 100%;
    }
  }

  & .modal-footer {
    width: 100%;
    display: flex;
    justify-content: space-between;
    padding-top: 2rem;

    & .action-btn {
      width: 9rem;
      height: 4rem;
      display: flex;
      align-items: center;
      justify-content: center;
      text-align: center;
    }
  }
`

const MessagesInitialFilters = {
  languageCode: '',
  key: '',
  value: ''
}

const Messages = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const LanguageCodeOptions = [
    { value: '', name: t('None') },
    { value: 'us', name: t('us') },
    { value: 'bs', name: t('bs') }
  ]

  const modalRef = useRef()
  const filterModalRef = useRef()

  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10
  })

  const [editMessage, setEditMessage] = useState({})
  const [filters, setFilters] = useState(MessagesInitialFilters)

  const [modalFilters, setModalFilters] = useState(MessagesInitialFilters)

  const TableColumns = [
    { id: 1, width: '30%', value: t('Language'), accessor: 'languageCode' },
    { id: 2, width: '30%', value: t('Key'), accessor: 'key' },
    { id: 3, width: '30%', value: t('Value'), accessor: 'value' },
    {
      id: 4,
      width: '10%',
      value: t('Actions'),
      render: (params) => (
        <div
          onClick={() => {
            setEditMessage(params)
            modalRef?.current?.showModal()
          }}
        >
          <IconEdit size="m" />
        </div>
      )
    }
  ]

  const { data, isSuccess, isLoading, refetch } = useMessages({
    page: pagination.page,
    pageSize: pagination.pageSize,
    //enabled: !isEmpty(filters),
    ...filters
  })

  const saveCommand = useSaveMessage(
    () => {
      refetch()
      modalRef?.current?.close()
      setEditMessage({})
    },
    () => {}
  )

  const updateEditMessage = (accessor, value) => {
    setEditMessage((p) => {
      return {
        ...p,
        [`${accessor}`]: value
      }
    })
  }

  const onSaveChanges = () => {
    saveCommand.mutate(editMessage)
  }

  const onFilterChange = (accessor, value) => {
    setModalFilters((f) => {
      return {
        ...f,
        [accessor]: value
      }
    })
  }

  const onApplyFilters = () => {
    setFilters(modalFilters || MessagesInitialFilters)
    setPagination((p) => {
      return { ...p, page: 1 }
    })

    filterModalRef?.current?.close()
  }

  const onClearFilters = () => {
    setModalFilters(MessagesInitialFilters)
  }

  return (
    <Container>
      <FullPageLoader loading={isLoading} />

      <div className="header">
        <FilterButton
          filters={filters}
          onClick={() => filterModalRef?.current?.showModal()}
          //disabled={!isFilterApplied && !isLoading && data?.totalCount === 0}
        >
          {t('Filters')}
        </FilterButton>
      </div>

      <div className="table-content">
        {isSuccess ? (
          <>
            <Table data={data?.items} zebra={true} columns={TableColumns} />
            <div className="table-footer">
              <div className="items-per-page">
                {data?.totalCount > 0 && (
                  <ItemsPerPage
                    totalCount={data?.totalCount}
                    page={pagination.page}
                    value={pagination.pageSize}
                    onChange={({ value }) => {
                      setPagination({ page: 1, pageSize: value })
                    }}
                  />
                )}
              </div>
              <div>
                {data?.totalCount > 0 && (
                  <Pagination
                    className="pagination"
                    currentPage={pagination.page}
                    totalPages={Math.ceil(data?.totalCount / pagination.pageSize)}
                    onPage={(x) =>
                      setPagination((p) => {
                        return {
                          ...p,
                          page: x
                        }
                      })
                    }
                  />
                )}
              </div>
            </div>
          </>
        ) : (
          <div className="no-data-label">{t('No data')}</div>
        )}
      </div>

      <div className="pts-footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={() => navigate(-1)}
          shadow={true}
        />
      </div>

      {/* Edit message modal */}
      <Modal ref={modalRef} onClose={() => modalRef?.current?.close()}>
        <ModalContent>
          <div className="modal-header">{t('Edit message')}</div>
          <div className="modal-content">
            <TextInput label={t('Key')} value={editMessage?.key} disabled={true} />
            <Divider height={'0.25rem'} />
            <TextInput label={t('Language')} value={editMessage?.languageCode} disabled={true} />
            <Divider height={'0.25rem'} />
            <TextInput
              label={t('Value')}
              value={editMessage?.value}
              onChange={({ target }) => updateEditMessage('value', target.value)}
            />
            <Divider height={'0.25rem'} />
          </div>

          <div className="modal-footer">
            <Button className="action-btn" onClick={() => modalRef?.current?.close()}>
              {t('Cancel')}
            </Button>
            <Button className="action-btn" onClick={onSaveChanges}>
              {t('Save')}
            </Button>
          </div>
        </ModalContent>
      </Modal>

      {/* Filters modal */}
      <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
        <ModalContent>
          <div className="modal-header">{t('Filters')}</div>
          <div className="modal-content">
            <DropdownInput
              label={t('Language')}
              options={LanguageCodeOptions}
              value={modalFilters?.languageCode}
              onChange={(option) => onFilterChange('languageCode', option?.value)}
              className="dropdown"
            />
            <Divider height={'0.25rem'} />
            <TextInput
              size="m"
              label={t('Key')}
              value={modalFilters?.key}
              placeholder={t('Key')}
              onChange={({ target }) => onFilterChange('key', target?.value)}
            />
            <Divider height={'0.25rem'} />
            <TextInput
              size="m"
              label={t('Value')}
              value={modalFilters?.value}
              placeholder={t('Value')}
              onChange={({ target }) => onFilterChange('value', target?.value)}
            />
            <Divider height={'0.25rem'} />
          </div>

          <div className="modal-footer">
            <Button className="action-btn" onClick={onClearFilters} color="light">
              {t('Clear')}
            </Button>
            <Button className="action-btn" onClick={onApplyFilters}>
              {t('Apply')}
            </Button>
          </div>
        </ModalContent>
      </Modal>
    </Container>
  )
}

export default Messages
