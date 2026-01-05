/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */

import { useOperators } from '@domain/operator/queries'
import {
  useSaveOperator,
  useAddOperator,
  useChangeOperatorPassword,
  useActivateCard
} from '@domain/operator/commands'
import { useRef, useState } from 'react'
import { usePermissions } from '@domain/operator/queries'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import useRefreshOpertorToken from '@domain/operator/hooks/use-refresh-operator-token'
import Button from '@ui/components/button'
import Modal from '@ui/components/modal'
import TextInput from '@ui/components/inputs/text-input'
import Divider from '@ui/components/divider'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import CircleButton from '@ui/components/circle-button'
import { useCardsForOperator } from '@domain/operator/queries'
import Table from '@ui/components/table'
import { preparePermissionsData } from '@domain/administration/services'
import FullPageLoader from '@ui/components/full-page-loader'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'
import IconLock from '@ui/components/icons/IconLock'
import IconBankCard from '@ui/components/icons/IconBankCard'
import IconTrashCan from '@ui/components/icons/IconTrashCan'
import { isEmpty } from 'lodash'
import Pagination from '@ui/components/pagination'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/operator/commands'
import { useCloseCard } from '@domain/operator/commands'
import ConfirmationDialog from '@ui/components/confirmation-dialog'
import { formatDateString } from '@domain/administration/services'
import FilterButton from '@ui/components/filter-button'
import ItemsPerPage from '@ui/components/items-per-page'
import CardReaderStepsContent from './card-reader-steps'
import { useCardStoreActions } from '@src/app/domain/administration/stores/card-reader'
import Toggle from '@ui/components/toggle'
import { useIsCardReaderReady } from '@src/app/domain/device/stores'
import { useNavigate } from 'react-router-dom'

const Actions = styled.div`
  padding: 1rem 0;
  display: flex;
  flex-direction: row-reverse;
  gap: 1.25rem;
  margin-right: 0.75rem;
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
  }

  & .card-modal-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    text-align: center;
    padding-bottom: 3rem;

    & .step {
      width: 100%;
      align-items: center;
      display: flex;
      flex-direction: column;

      & .disabled {
        color: var(--text-medium);
      }

      & .success {
        color: var(--primary-medium);
        font-weight: 600;
      }

      & .failed {
        color: var(--error-dark);
        font-weight: 600;
      }
    }

    & .retry-btn {
      margin-top: 1rem;
      width: 10rem;
      height: 3.5rem;
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

const OperatorInfoContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
  padding-bottom: 3rem;
  height: 100%;
  overflow: auto;

  & .header {
    margin-top: 4.25rem;
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
  }
  & .heading-label {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    font-family: Poppins;
  }
  & .heading-label-m {
    margin-bottom: 0.5rem;
  }
  & .flex-row {
    display: flex;
    align-items: center;
    gap: 1rem;
    width: 100%;

    > div {
      width: 100%;
    }
  }
  & .details {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  & .permissions {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .checkbox-wrapper {
      display: flex;
      gap: 1.25rem;
      width: 100%;
      padding: 1rem 0 1rem 2rem;
    }
  }
  & .footer {
    margin-top: auto;
    display: flex;
    gap: 4rem;
    width: 100%;
    justify-content: space-between;
    padding-bottom: 3rem !important;
  }
`

const ExternalWrapper = styled.div`
  //height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;

  & .pts-footer {
    margin: 2rem 0 0 0;
    display: flex;
    gap: 4rem;
    width: 100%;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;
  }
`

const Container = styled.div`
  height: inherit;
`

const OperatorCardsContainer = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;

  & .card-label {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    font-family: Poppins;
  }

  & .cards-header {
    display: flex;
    justify-content: space-between;
    padding: 1rem 0;

    & .action-btn {
      width: 10rem;
      height: 4rem;
      display: flex;
      align-items: center;
      justify-content: center;
      text-align: center;
    }
    & .breadcrumbs {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
  }

  & .cards-table {
    padding: 1.5rem 0;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    flex: 1;

    td {
      text-align: center;
    }

    & .no-data-label {
      text-align: center;
      padding: 2rem;
      color: var(--text-inactive-dark);
    }

    & .card-table-footer {
      display: flex;
      gap: 2rem;
      align-self: center;
      margin-bottom: 2rem;
      padding-top: 1.5rem;

      & .items-per-page {
        > div {
          flex-direction: column;
          align-items: center;
        }
      }

      & .pagination {
        align-self: end;
      }

      @media screen and (max-width: 800px) {
        flex-direction: column-reverse;
        align-items: center;
        gap: 0.5rem;
      }
    }
  }

  & .footer {
    margin-top: auto;
    display: flex;
    gap: 4rem;
    width: 100%;
    justify-content: space-between;
    padding-bottom: 2.5rem;
    position: sticky;
    bottom: 0;
    z-index: 10;
  }
`

const OperatorsTableContainter = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .actions {
    gap: 2rem;
    display: flex;
  }

  td {
    text-align: center;
    width: 100%;
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

const OperatorInitFilters = {
  firstName: '',
  lastName: ''
}

const Operators = ({ setHideTabs }) => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const [operator, setOperator] = useState({})

  /* Password props */
  const passwordModalRef = useRef()

  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  /* Operator info modal props */
  const [operatorOpen, setOperatorOpen] = useState(false)
  const [infoModalObject, setInfoModalObject] = useState()

  const setOperatorInfoData = (accessor, value) => {
    setInfoModalObject((p) => {
      return { ...p, [`${accessor}`]: value }
    })
  }

  const onPermissionClick = (permission) => {
    const op = infoModalObject?.permissions?.map((x) => x.code)

    if (op && op?.includes(permission.code)) {
      setOperatorInfoData('permissions', [
        ...(infoModalObject?.permissions?.filter((x) => x.code != permission.code) || [])
      ])
    } else {
      setOperatorInfoData('permissions', [...(infoModalObject?.permissions || []), permission])
    }
  }

  const onSaveOperator = () => {
    if (infoModalObject?.id) {
      saveOperatorCommand.mutate(infoModalObject)
    } else {
      addOperatorCommand.mutate(infoModalObject)
    }

    setHideTabs(false)
  }

  const onAddOperatorClick = () => {
    setOperatorOpen(true)
    setHideTabs(true)
    setInfoModalObject(null)
  }

  /* Cards modal props */
  const [cardsOpen, setCardsOpen] = useState(false)
  const [cardsPagination, setCardsPagination] = useState({
    page: 1,
    pageSize: 10
  })
  const [activeCard, setActiveCard] = useState({})

  const isCardReaderReady = useIsCardReaderReady()

  const addCardModalRef = useRef()
  const cardRemoveDialogRef = useRef()

  const operatorCardsQuery = useCardsForOperator({
    operatorId: operator?.id,
    ...cardsPagination,
    enabled: cardsOpen
  })

  const deactivateCard = useCloseCard(operator?.id, () => cardRemoveDialogRef?.current?.close())
  const activateCard = useActivateCard(operator?.id, () => cardRemoveDialogRef?.current?.close())

  /* Table columns definitions */
  const CardTableColumns = [
    { id: 1, width: null, value: t('No.'), accessor: 'serialNumber' },
    {
      id: 2,
      width: null,
      value: t('Issued by'),
      render: (params) => <div>{params.issuedBy || '/'}</div>
    },
    {
      id: 3,
      width: null,
      value: t('Issued at'),
      render: (params) => <div>{formatDateString(params.issuedAt)}</div>
    },
    {
      id: 4,
      width: null,
      value: t('Valid from-to'),
      render: (params) => (
        <span>
          <span>{`${formatDateString(params.validFrom)} - `}</span>
          <span>{`${formatDateString(params.validTo)}`}</span>
        </span>
      )
    },
    {
      id: 6,
      width: 'fit-content',
      value: t('Active'),
      render: (params) => (
        <>
          <Toggle
            checked={params?.status?.code === 'active'}
            className="toggle"
            onClick={(e) => {
              e.stopPropagation()

              setActiveCard(params)
              cardRemoveDialogRef?.current?.showModal()
            }}
          />
        </>
      )
    }
  ]

  const OperatorTableColumns = [
    { id: 1, width: null, value: t('Username'), accessor: 'username' },
    {
      id: 2,
      width: null,
      value: t('First Last Name'),
      render: (x) => `${x.firstName} ${x.lastName}`
    },
    { id: 3, width: null, value: t('Email'), accessor: 'email' },
    {
      id: 4,
      width: '20%',
      value: t('Actions'),
      render: (params) => (
        <div className="actions">
          <div
            onClick={(e) => {
              e.stopPropagation()
              onRowClick(params)
            }}
          >
            <IconInfoSquare size="s" />
          </div>
          <div
            onClick={(e) => {
              e.stopPropagation()
              onCardsClick(params)
            }}
          >
            <IconBankCard size="s" />
          </div>
          <div
            onClick={(e) => {
              e.stopPropagation()
              onPasswordClick(params)
            }}
          >
            <IconLock size="s" color="black" />
          </div>
        </div>
      )
    }
  ]

  /* Other props */

  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10
  })

  const filterModalRef = useRef()

  const [filters, setFilters] = useState(OperatorInitFilters)
  const [filterModalData, setFilterModalData] = useState(OperatorInitFilters)

  const { refreshToken } = useRefreshOpertorToken({ id: operator?.id })

  const operatorsQuery = useOperators({ ...pagination, ...filters })

  /* All permissions */
  const permissionsQuery = usePermissions({ enabled: operator?.id && operatorOpen })

  const { reset: resetCardScanProcess, setOperator: setCardsOperator } = useCardStoreActions()

  const saveOperatorCommand = useSaveOperator(() => {
    setOperatorOpen(false)
    setHideTabs(false)
    setOperator((p) => {
      return {
        ...p,
        ...infoModalObject
      }
    })
    operatorsQuery?.refetch()

    refreshToken()
  })

  const closePasswordModal = () => {
    passwordModalRef?.current?.close()
  }

  const onFilterChange = (accessor, value) => {
    setFilterModalData((f) => {
      return {
        ...f,
        [accessor]: value
      }
    })
  }

  const onApplyFilters = () => {
    setFilters(filterModalData)
    setPagination((p) => {
      return { ...p, page: 1 }
    })

    filterModalRef?.current?.close()
  }

  const onClearFilters = () => {
    setFilterModalData(OperatorInitFilters)
  }

  const changePasswordCommand = useChangeOperatorPassword(closePasswordModal)

  const addOperatorCommand = useAddOperator(() => {
    setOperatorOpen(false)
    operatorsQuery?.refetch()
  })

  const isLoading =
    operatorsQuery?.isLoading ||
    permissionsQuery?.isLoading ||
    saveOperatorCommand?.isPending ||
    changePasswordCommand?.isPending ||
    addOperatorCommand?.isPending ||
    operatorCardsQuery?.isLoading

  //=======> Methods

  const onCardsClick = (row) => {
    setOperator(row)
    setCardsOperator(row.id)

    setCardsOpen(true)
  }

  const onPasswordClick = (row) => {
    setOperator(row)

    setNewPassword('')
    setConfirmPassword('')

    passwordModalRef?.current?.showModal()
  }

  const onRowClick = (row) => {
    setOperator(row)
    setInfoModalObject(row)

    setHideTabs(true)
    setOperatorOpen(true)
  }

  const onSavePassword = () => {
    const data = {
      operatorId: operator.id,
      password: newPassword,
      confirmPassword: confirmPassword
    }

    changePasswordCommand.mutate(data)
  }

  const onCreateCard = () => {
    Mediator.dispatch(CommandType.InitializeCardReader, {
      id: operator.id
    })
    resetCardScanProcess()
    addCardModalRef?.current?.showModal()
  }

  const onCardModalClose = () => {
    operatorCardsQuery?.refetch()
    addCardModalRef?.current?.close()
  }

  //=======> Render

  return (
    <ExternalWrapper>
      <Container>
        <FullPageLoader loading={isLoading} />

        {/* Edit password modal */}
        <Modal ref={passwordModalRef} onClose={closePasswordModal}>
          <ModalContent>
            <div className="modal-header">{t('Change password')}</div>
            <div className="modal-content">
              <TextInput
                size="m"
                isPassword={true}
                placeholder={t('Enter new password')}
                label={t('New password')}
                value={newPassword}
                onChange={({ target }) => setNewPassword(target?.value)}
              />
              <Divider height={'0.25rem'} className="text-divider" />
              <TextInput
                size="m"
                isPassword={true}
                placeholder={t('Confirm password')}
                label={t('Confirm password')}
                value={confirmPassword}
                onChange={({ target }) => setConfirmPassword(target?.value)}
              />
              <Divider height={'0.25rem'} className="text-divider" />
            </div>
            <div className="modal-footer">
              <Button className="action-btn" onClick={closePasswordModal}>
                {t('Cancel')}
              </Button>
              <Button className="action-btn" onClick={onSavePassword}>
                {t('Save')}
              </Button>
            </div>
          </ModalContent>
        </Modal>

        {/* Edit operator modal */}
        {operatorOpen && (
          <OperatorInfoContainer>
            <div className="header">
              <div className="heading-label">
                {isEmpty(infoModalObject) ? t('Add operator') : t('Edit operator')}
              </div>
              <Divider height="0.25rem" />
            </div>

            <div className="details">
              <div className="heading-label heading-label-m">{t('Personal details')}</div>
              <div className="flex-row">
                <TextInput
                  placeholder={t('Username')}
                  label={t('Username')}
                  value={infoModalObject?.username}
                  onChange={({ target }) => setOperatorInfoData('username', target?.value)}
                />
                <TextInput
                  placeholder={t('Email')}
                  label={t('Email')}
                  value={infoModalObject?.email}
                  onChange={({ target }) => setOperatorInfoData('email', target?.value)}
                />
              </div>
              <Divider height="0.25rem" />
              <div className="flex-row">
                <TextInput
                  placeholder={t('First name')}
                  label={t('First name')}
                  value={infoModalObject?.firstName}
                  onChange={({ target }) => setOperatorInfoData('firstName', target?.value)}
                />
                <TextInput
                  placeholder={t('Last name')}
                  label={t('Last name')}
                  value={infoModalObject?.lastName}
                  onChange={({ target }) => setOperatorInfoData('lastName', target?.value)}
                />
              </div>
              <Divider height="0.25rem" />
              <div className="flex-row">
                <TextInput
                  placeholder={t('Phone number')}
                  label={t('Phone number')}
                  value={infoModalObject?.phoneNumber}
                  onChange={({ target }) => setOperatorInfoData('phoneNumber', target?.value)}
                />
                <TextInput
                  placeholder={t('Remarks')}
                  label={t('Remarks')}
                  value={infoModalObject?.remarks}
                  onChange={({ target }) => setOperatorInfoData('remarks', target?.value)}
                />
              </div>
              <Divider height="0.25rem" />
              <div className="flex-row">
                <TextInput
                  placeholder={t('Company')}
                  label={t('Company')}
                  value={infoModalObject?.company}
                  onChange={({ target }) => setOperatorInfoData('company', target?.value)}
                />
              </div>
            </div>

            <div className="permissions">
              <div className="heading-label heading-label-m">{t('Permissions')}</div>
              {preparePermissionsData(
                infoModalObject?.permissions,
                permissionsQuery?.data?.permissions
              )?.map((e, i) => (
                <>
                  <div key={`permission-row-${i}`} className="flex-row">
                    {e?.map((p, pi) => (
                      <div
                        key={`permission-${pi}-${i * pi}`}
                        className="checkbox-wrapper"
                        onClick={() => onPermissionClick(p)}
                      >
                        <CheckboxInput
                          value={p.active}
                          /* onChange={() => {
                            onPermissionClick(p)
                          }} */
                        />
                        <div className="heading-label">{t(p.code)}</div>
                      </div>
                    ))}
                  </div>
                  <Divider height="0.25rem" />
                </>
              ))}
            </div>
          </OperatorInfoContainer>
        )}

        {/* Filters modal */}
        <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
          <ModalContent>
            <div className="modal-header">{t('Filters')}</div>
            <div className="modal-content">
              <TextInput
                size="m"
                placeholder={t('Enter first name')}
                label={t('First name')}
                value={filterModalData?.firstName}
                onChange={({ target }) => {
                  onFilterChange('firstName', target?.value)
                }}
              />
              <Divider height={'0.25rem'} className="text-divider" />
              <TextInput
                size="m"
                placeholder={t('Enter last name')}
                label={t('Last name')}
                value={filterModalData?.lastName}
                onChange={({ target }) => {
                  onFilterChange('lastName', target?.value)
                }}
              />
              <Divider height={'0.25rem'} className="text-divider" />
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

        {/* Operator cards page */}
        {cardsOpen && (
          <OperatorCardsContainer>
            <div className="cards-header">
              <div className="breadcrumbs">
                <div className="card-label">{t('Cards')}</div>
                <IconRightHalfArrow size="m" />
                <div className="card-label">{`${operator?.firstName} ${operator?.lastName}`}</div>
              </div>
              <Button
                className="action-btn"
                onClick={onCreateCard}
                disabled={isCardReaderReady === false}
              >
                {t('Add card')}
              </Button>
            </div>

            {operatorCardsQuery?.isLoading === false && (
              <div className="cards-table">
                {operatorCardsQuery?.data?.cards && operatorCardsQuery?.data?.totalCount > 0 ? (
                  <>
                    <Table
                      columns={CardTableColumns}
                      data={operatorCardsQuery?.data?.cards}
                      zebra={true}
                    />
                    <div className="card-table-footer">
                      <div className="items-per-page">
                        <ItemsPerPage
                          totalCount={operatorCardsQuery?.data?.totalCount}
                          page={cardsPagination.page}
                          value={cardsPagination.pageSize}
                          onChange={({ value }) => {
                            setCardsPagination({ page: 1, pageSize: value })
                          }}
                        />
                      </div>

                      <Pagination
                        className="pagination"
                        currentPage={cardsPagination.page}
                        totalPages={Math.ceil(
                          operatorCardsQuery?.data?.totalCount / cardsPagination.pageSize
                        )}
                        onPage={(x) =>
                          setCardsPagination((p) => {
                            return {
                              ...p,
                              page: x
                            }
                          })
                        }
                      />
                    </div>
                  </>
                ) : (
                  <div className="card-label no-data-label">{t('No data')}</div>
                )}
              </div>
            )}

            <div className="footer">
              <CircleButton
                icon={(props) => <IconLeftHalfArrow {...props} />}
                size="l"
                color="dark"
                textRight={t('Back')}
                onClick={() => {
                  setCardsOpen(false)
                }}
              />
            </div>
          </OperatorCardsContainer>
        )}

        {/* Card delete cofirmation dialog */}
        <ConfirmationDialog
          ref={cardRemoveDialogRef}
          onCancel={() => cardRemoveDialogRef?.current?.close()}
          header={
            activeCard?.status?.code === 'active'
              ? t('Are you sure you want to deactivate this card?')
              : t('Are you sure you want to activate this card?')
          }
          onConfirm={() => {
            if (activeCard?.id) {
              if (activeCard?.status?.code === 'active') {
                deactivateCard.mutate({
                  operatorId: operator.id,
                  id: activeCard.id,
                  cardId: activeCard.id
                })
              } else {
                activateCard.mutate({
                  operatorId: operator.id,
                  id: activeCard.id,
                  cardId: activeCard.id
                })
              }
            }
          }}
        />

        {/* Add new card modal */}
        <Modal ref={addCardModalRef} onClose={onCardModalClose}>
          <ModalContent>
            <div className="modal-header">{t('Adding new card')}</div>
            <div className="card-modal-content">
              <CardReaderStepsContent />
            </div>
          </ModalContent>
        </Modal>

        {/* Operators table */}
        {!operatorOpen && !cardsOpen && (
          <>
            <Actions>
              <FilterButton
                filters={filters}
                onClick={() => filterModalRef?.current?.showModal()}
                //disabled={!isFilterApplied && !isLoading && operatorsQuery?.data?.totalCount === 0}
              >
                {t('Filters')}
              </FilterButton>
              <Button onClick={onAddOperatorClick}>{t('Add operator')}</Button>
            </Actions>

            <OperatorsTableContainter>
              <Table
                columns={OperatorTableColumns}
                data={operatorsQuery?.data?.operators || []}
                zebra={true}
                onRowClick={(e, rowData) => {
                  onRowClick(rowData)
                }}
              />
              <div className="table-footer">
                <div className="items-per-page">
                  {operatorsQuery?.data?.totalCount > 0 && (
                    <ItemsPerPage
                      totalCount={operatorsQuery?.data?.totalCount}
                      page={pagination.page}
                      value={pagination.pageSize}
                      onChange={({ value }) => {
                        setPagination({ page: 1, pageSize: value })
                      }}
                    />
                  )}
                </div>

                {operatorsQuery?.data?.totalCount > 0 && (
                  <Pagination
                    className="pagination"
                    currentPage={pagination.page}
                    totalPages={Math.ceil(operatorsQuery?.data?.totalCount / pagination.pageSize)}
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
            </OperatorsTableContainter>
          </>
        )}
      </Container>

      {/* Footer */}
      {!operatorOpen && !cardsOpen && (
        <div className="pts-footer">
          <CircleButton
            icon={(props) => <IconLeftHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Back')}
            onClick={() => navigate(-1)}
          />
        </div>
      )}

      {operatorOpen && (
        <div className="pts-footer">
          <CircleButton
            icon={(props) => <IconLeftHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Back')}
            onClick={() => {
              setOperatorOpen(false)
              setHideTabs(false)
            }}
            shadow={true}
          />

          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            onClick={onSaveOperator}
            disabled={false}
            shadow={true}
          />
        </div>
      )}
    </ExternalWrapper>
  )
}

export default Operators
