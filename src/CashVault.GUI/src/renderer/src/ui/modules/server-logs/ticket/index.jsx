/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Table from '@ui/components/table'
import { useTranslation } from '@domain/administration/stores'
import { useEffect, useRef, useState } from 'react'
import { useWindowSize } from 'react-use'
import { getClosestPageSize } from '@src/app/services/utils'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'
import Pagination from '@ui/components/pagination'
import FullPageLoader from '@ui/components/full-page-loader'
import DateTimeCellRenderer from '@ui/components/cell-renders/date-time-cell-renderer'
import {
  useTicketLogs,
  useTicketLogDetails,
  getTicketLogDetailsData
} from '@domain/records/queries'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'
import Divider from '@ui/components/divider'
import TextInput from '@ui/components/inputs/text-input'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import FilterButton from '@ui/components/filter-button'
import DateInput from '@ui/components/inputs/date-input'
import ItemsPerPage from '@ui/components/items-per-page'
import TicketDetails from '../ticket-details'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .table {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  & .filter {
    padding: 1rem 0;
    display: flex;
    flex-direction: row-reverse;
    margin-right: 0.75rem;
  }

  & .table-footer {
    display: flex;
    gap: 2rem;
    justify-content: center;
    align-items: flex-end;
    margin-bottom: 2rem;
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
`

const ModalContent = styled.div`
  & .header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }

  & .content {
    padding-top: 2rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .row {
      width: 100%;
      display: flex;
      gap: 1rem;
    }

    & .full-width {
      width: 100%;
    }

    & .flex-row {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    & .checkbox-wrapper {
      display: flex;
      gap: 1.25rem;
      width: 100%;
      padding: 1rem 0 1rem 2rem;
    }

    & .heading-label {
      font-weight: 600;
      font-size: 1.75rem;
      line-height: 1.875rem;
      font-family: Poppins;
    }
  }

  & .footer {
    width: 100%;
    display: flex;
    justify-content: space-between;
    padding-top: 2rem;

    //position: absolute;
    //bottom: 0;
    //left: 0;
    //right: 0;
    //margin: 2.375rem 3.125rem;
    //width: inherit;

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

const InitialFilters = {
  barcode: '',
  amountFrom: null,
  amountTo: null,
  printingRequestedAtFrom: null,
  printingRequestedAtTo: null,
  printingCompletedAtFrom: null,
  printingCompletedAtTo: null,
  typeId: -1,
  daysValidFrom: null,
  daysValidTo: null,
  isLocal: null,
  isPrinted: null,
  isRedeemed: null,
  isStacked: null,
  isExpired: null
}

const TicketLogs = () => {
  const { t } = useTranslation()
  const heightOffset = 250
  const rowHeight = 80

  //=======> State

  const windowSize = useWindowSize()

  const filterModalRef = useRef()
  const modalDetailsRef = useRef()

  const [filters, setFilters] = useState(InitialFilters)
  const [totalRowCount, setTotalRowCount] = useState(0)
  const [details, setLogDetails] = useState(null)
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10
  })

  const [modalFilters, setModalFilters] = useState(InitialFilters)

  //=======> Queries

  const { data, isSuccess, isLoading } = useTicketLogs({
    page: pagination.page,
    pageSize: pagination.pageSize,
    enabled: true,
    ...filters
  })

  //=======> Lifecycle

  useEffect(() => {
    if (data?.totalCount && totalRowCount !== data?.totalCount) {
      setTotalRowCount(data?.totalCount)
    }
  }, [data, isSuccess, isLoading])

  useEffect(() => {
    var height = windowSize.height - heightOffset
    var numberOfRows = Math.round(height / rowHeight)

    setPagination({ ...pagination, pageSize: getClosestPageSize(numberOfRows) })
  }, [])

  //=======> Methods

  const onDetailsClick = async (ticketId) => {
    var details = await getTicketLogDetailsData({ id: ticketId })
    setLogDetails(details)
    modalDetailsRef.current.showModal()
  }

  const onFilterChange = (accessor, value) => {
    setModalFilters((f) => {
      return {
        ...f,
        [accessor]: value
      }
    })
  }

  const onCheckboxFilter = (accessor, checked) => {
    if (!checked) {
      checked = null
    }

    onFilterChange(accessor, checked)
  }

  const onApplyFilters = () => {
    setFilters(modalFilters)
    setPagination((p) => {
      return { ...p, page: 1 }
    })

    filterModalRef?.current?.close()
  }

  const onClearFilters = () => {
    setModalFilters(InitialFilters)
  }

  const onCloseModalDetails = () => {
    modalDetailsRef.current.close()
    setLogDetails(null)
  }

  //=======> Render

  const columns = [
    {
      id: 1,
      width: null,
      value: t('Number'),
      accessor: 'number'
    },
    { id: 2, width: null, value: t('Amount'), accessor: 'amount' },
    { id: 3, width: '30%', value: `${t('Barcode')}`, accessor: 'barcode' },
    {
      id: 4,
      width: null,
      value: `${t('Type')}`,
      render: (rowData) => <div>{rowData?.type?.code}</div>
    },
    {
      id: 5,
      width: null,
      value: t('Timestamp'),
      render: (rowData) => {
        return <DateTimeCellRenderer value={rowData?.created} />
      }
    },

    {
      id: 6,
      width: '8%',
      value: t(''),
      render: (rowData) => (
        <div className="info-icon" onClick={() => onDetailsClick(rowData?.id)}>
          <IconInfoSquare />
        </div>
      )
    }
  ]

  return (
    <Container>
      <FullPageLoader loading={isLoading} />

      <div className="filter">
        <FilterButton
          //disabled={!isLoading && data?.totalCount === 0}
          onClick={() => filterModalRef?.current?.showModal()}
          filters={filters}
        >
          {t('Filters')}
        </FilterButton>
      </div>

      <div className="table">
        <Table columns={columns} data={data?.tickets || []} zebra={true} />
      </div>

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
        {data?.totalCount > 0 && (
          <div className="pagination">
            <Pagination
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
          </div>
        )}
      </div>

      <TicketDetails ref={modalDetailsRef} data={details} onClose={onCloseModalDetails} />

      {/* Filters modal */}
      <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
        <ModalContent>
          <div className="header">{t('Filter Logs')}</div>
          <div className="content">
            <div className="row">
              <TextInput
                size="m"
                className="full-width"
                label={t('Barcode')}
                value={modalFilters?.barcode ?? ''}
                placeholder={t('Enter barcode')}
                onChange={({ target }) => onFilterChange('barcode', target?.value)}
              />

              <DropdownInput
                size="m"
                label={t('Type')}
                className="full-width"
                options={[{ id: -1, code: t('None') }, ...(data?.ticketTypes || [])]}
                value={modalFilters?.typeId ?? ''}
                optionLabelName="code"
                optionValueName="id"
                onChange={(option) => onFilterChange('typeId', option.id)}
              />
            </div>

            <Divider height={'0.15rem'} />

            <div className="row">
              <DecimalInput
                size="m"
                className="full-width"
                label={t('Amount from')}
                value={modalFilters?.amountFrom ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountFrom', values?.float)
                }}
                onClear={() => onFilterChange('amountFrom', null)}
              />
              <DecimalInput
                size="m"
                className="full-width"
                label={t('Amount to')}
                value={modalFilters?.amountTo ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountTo', values?.float)
                }}
                onClear={() => onFilterChange('amountTo', null)}
              />
            </div>
            <Divider height={'0.15rem'} />

            <div className="row">
              <DateInput
                size="m"
                className="full-width"
                label={t('Printing requested timestamp from')}
                value={modalFilters?.printingRequestedAtFrom ?? ''}
                dateFormat={'dd-MM-yyyy'}
                onChange={(value) => onFilterChange('printingRequestedAtFrom', value)}
              />
              <DateInput
                size="m"
                className="full-width"
                label={t('Printing requested timestamp to')}
                value={modalFilters?.printingRequestedAtTo ?? ''}
                dateFormat={'dd-MM-yyyy'}
                onChange={(value) => onFilterChange('printingRequestedAtTo', value)}
              />
            </div>
            <Divider />

            <div className="row">
              <DateInput
                size="m"
                className="full-width"
                label={t('Printing completed timestamp from')}
                value={modalFilters?.printingCompletedAtFrom ?? ''}
                dateFormat={'dd-MM-yyyy'}
                onChange={(value) => onFilterChange('printingCompletedAtFrom', value)}
              />
              <DateInput
                size="m"
                className="full-width"
                label={t('Printing completed timestamp to')}
                value={modalFilters?.printingCompletedAtTo ?? ''}
                dateFormat={'dd-MM-yyyy'}
                onChange={(value) => onFilterChange('printingCompletedAtTo', value)}
              />
            </div>
            <Divider />

            <div className="row">
              <DecimalInput
                size="m"
                className="full-width"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Days valid from')}
                value={modalFilters?.daysValidFrom ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('daysValidFrom', values?.float)
                }}
                onClear={() => onFilterChange('daysValidFrom', null)}
              />
              <DecimalInput
                size="m"
                className="full-width"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Days valid to')}
                value={modalFilters?.daysValidTo ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('daysValidTo', values?.float)
                }}
                onClear={() => onFilterChange('daysValidTo', null)}
              />
            </div>
            <Divider height={'0.15rem'} />

            <div className="flex-row">
              <div className="checkbox-wrapper">
                <CheckboxInput
                  value={modalFilters?.isLocal ?? false}
                  onChange={({ target }) => onCheckboxFilter('isLocal', target.checked)}
                />
                <div className="heading-label">{t('Local')}</div>
              </div>

              <div className="checkbox-wrapper">
                <CheckboxInput
                  value={modalFilters?.isPrinted ?? false}
                  onChange={({ target }) => onCheckboxFilter('isPrinted', target.checked)}
                />
                <div className="heading-label">{t('Printed')}</div>
              </div>

              <div className="checkbox-wrapper">
                <CheckboxInput
                  value={modalFilters?.isRedeemed ?? false}
                  onChange={({ target }) => onCheckboxFilter('isRedeemed', target.checked)}
                />
                <div className="heading-label">{t('Redeemed')}</div>
              </div>
            </div>

            <div className="flex-row">
              <div className="checkbox-wrapper">
                <CheckboxInput
                  value={modalFilters?.isStacked ?? false}
                  onChange={({ target }) => onCheckboxFilter('isStacked', target.checked)}
                />
                <div className="heading-label">{t('Stacked')}</div>
              </div>

              <div className="checkbox-wrapper">
                <CheckboxInput
                  value={modalFilters?.isExpired ?? false}
                  onChange={({ target }) => onCheckboxFilter('isExpired', target.checked)}
                />
                <div className="heading-label">{t('Expired')}</div>
              </div>

              <div className="checkbox-wrapper" />
            </div>
          </div>
          <div className="footer">
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

export default TicketLogs
