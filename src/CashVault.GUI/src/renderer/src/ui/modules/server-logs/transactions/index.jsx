/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import Table from '@ui/components/table'
import { useTranslation } from '@domain/administration/stores'
import IconButtonCellRender from '@ui/components/cell-renders/icon-button-cell-render'
import AmountCellRender from '@ui/components/cell-renders/amount-cell-renderer'
import TransactionStatusCellRender from '@ui/components/cell-renders/transaction-status-cell-renderer'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'
import Divider from '@ui/components/divider'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { useRef, useState, useEffect } from 'react'
import { useTransactionLogs, getTransactionLogDetailsData } from '@domain/records/queries'
import DateTimeCellRenderer from '@ui/components/cell-renders/date-time-cell-renderer'
import Pagination from '@ui/components/pagination'
import FullPageLoader from '@ui/components/full-page-loader'
import FilterButton from '@ui/components/filter-button'
import DateInput from '@ui/components/inputs/date-input'
import ItemsPerPage from '@ui/components/items-per-page'
import TransactionDetails from '../transaction-details'
import { getClosestPageSize } from '@src/app/services/utils'
import { useWindowSize } from 'react-use'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

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
  height: 100%;
  display: flex;
  flex-direction: column;

  & .header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
    flex-shrink: 0;
  }

  & .content {
    padding-top: 2rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    flex: 1 1 auto;

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
  processingStartedDateFrom: null,
  processingStartedDateTo: null,
  processingEndedDateFrom: null,
  processingEndedDateTo: null,
  amountFrom: null,
  amountTo: null,
  amountRequestedFrom: null,
  amountRequestedTo: null,
  typeId: -1,
  statusId: -1,
  kind: null
}

const TransactionLogs = () => {
  const { t } = useTranslation()
  const modalDetailsRef = useRef()
  const heightOffset = 250
  const rowHeight = 80

  // =========> State

  const filterModalRef = useRef()
  const windowSize = useWindowSize()
  const [filters, setFilters] = useState(InitialFilters)

  const [modalFilters, setModalFilters] = useState(InitialFilters)
  const [details, setLogDetails] = useState(null)
  const [totalRowCount, setTotalRowCount] = useState(0)
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10
  })

  //=======> Queries

  const {
    data: transactionsData,
    isSuccess,
    isLoading
  } = useTransactionLogs({
    page: pagination.page,
    pageSize: pagination.pageSize,
    enabled: true,
    ...filters
  })

  useEffect(() => {
    var height = windowSize.height - heightOffset
    var numberOfRows = Math.round(height / rowHeight)

    setPagination({ ...pagination, pageSize: getClosestPageSize(numberOfRows) })
  }, [])

  useEffect(() => {
    if (transactionsData?.totalCount && totalRowCount !== transactionsData?.totalCount) {
      setTotalRowCount(transactionsData?.totalCount)
    }
  }, [transactionsData, isSuccess, isLoading])

  //==============> Methods

  const columns = [
    {
      id: 1,
      value: t('Time'),
      width: '20%',
      render: (rowData) => {
        //TODO: format dates

        return <DateTimeCellRenderer value={rowData?.processingStarted} />
      }
    },
    {
      id: 2,
      value: `${t('Type')} / ${t('Kind')}`,
      width: '20%',
      render: (rowData) => <div>{t(rowData?.kind)}</div>
    },
    {
      id: 3,
      value: t('Amount'),
      width: '25%',
      render: (rowData) => {
        var type = rowData?.type?.code?.toLowerCase()
        var status = rowData?.status?.code?.toLowerCase()

        return (
          <AmountCellRender
            type={type}
            status={status}
            //TODO: format amount
            amount={rowData?.amount}
            currency={rowData?.currency}
          />
        )
      }
    },
    {
      id: 4,
      value: t('Status'),
      width: '35%',
      render: (rowData) => {
        var status = rowData?.status?.code?.toLowerCase()
        var transactionType = rowData?.type?.code

        return (
          <TransactionStatusCellRender
            status={status}
            description={t(transactionType)}
            requestedAmount={rowData?.amountRequested}
          />
        )
      }
    },
    {
      id: 5,
      width: '10%',
      render: (rowData) => {
        return (
          <div className="info-icon">
            <IconButtonCellRender onClick={() => onOpenDetails(rowData)} />
          </div>
        )
      }
    }
  ]

  const rowColor = []

  transactionsData?.transactions?.forEach((x, i) => {
    if (x.status.code.toLowerCase() === 'failed') {
      rowColor.push({
        index: i,
        color: '#FFDCDC'
      })
    }

    if (x.status.code.toLowerCase() === 'partiallycompleted') {
      rowColor.push({
        index: i,
        color: '#F4E0CA'
      })
    }
  })

  //======> Methods

  const onOpenDetails = async (rowData) => {
    var result = await getTransactionLogDetailsData({
      id: rowData?.id,
      isMoneyStatusTransaction: false
    })
    setLogDetails(result)
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

  //======> Render

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

      <Table columns={columns} data={transactionsData?.transactions || []} rowColor={rowColor} />

      <div className="table-footer">
        <div className="items-per-page">
          {transactionsData?.totalCount > 0 && (
            <ItemsPerPage
              totalCount={transactionsData?.totalCount}
              page={pagination.page}
              value={pagination.pageSize}
              onChange={({ value }) => {
                setPagination({ page: 1, pageSize: value })
              }}
            />
          )}
        </div>

        {transactionsData?.totalCount > 0 && (
          <div className="pagination">
            <Pagination
              currentPage={pagination.page}
              totalPages={Math.ceil(transactionsData?.totalCount / pagination.pageSize)}
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

      <TransactionDetails ref={modalDetailsRef} data={details} onClose={onCloseModalDetails} />

      {/* Filters modal */}
      <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
        <ModalContent>
          <div className="header">{t('Filter Logs')}</div>
          <div className="content">
            <div className="row">
              <DropdownInput
                size="m"
                label={t('Status')}
                options={[
                  { id: -1, code: t('None') },
                  ...(transactionsData?.transactionStatuses || [])
                ]}
                value={modalFilters?.statusId}
                optionLabelName="code"
                optionValueName="id"
                onChange={(option) => onFilterChange('statusId', option.id)}
              />

              <DropdownInput
                size="m"
                label={t('Type')}
                options={[
                  { id: -1, code: t('None') },
                  ...(transactionsData?.transactionTypes || [])
                ]}
                value={modalFilters?.typeId}
                optionLabelName="code"
                optionValueName="id"
                onChange={(option) => onFilterChange('typeId', option.id)}
              />
            </div>
            <div className="row">
              <DropdownInput
                size="m"
                label={`${t('Type')} / ${t('Kind')}`}
                options={[
                  { value: null, name: t('None') },
                  ...(transactionsData?.transactionKindTypes || [])
                ]}
                value={modalFilters?.kind}
                optionLabelName="name"
                optionValueName="value"
                onChange={(option) => onFilterChange('kind', option.value)}
              />
            </div>

            <Divider height={'0.15rem'} />

            <div className="row">
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount from')}
                defaultValue={modalFilters?.amountFrom}
                value={modalFilters?.amountFrom}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountFrom', values?.float)
                }}
                onClear={() => onFilterChange('amountFrom', null)}
              />
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount to')}
                defaultValue={modalFilters?.amountTo}
                value={modalFilters?.amountTo}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountTo', values?.float)
                }}
                onClear={() => onFilterChange('amountTo', null)}
              />
            </div>
            <Divider height={'0.15rem'} />

            <div className="row">
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount requested from')}
                defaultValue={modalFilters?.amountRequestedFrom}
                value={modalFilters?.amountRequestedFrom}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountRequestedFrom', values?.float)
                }}
                onClear={() => onFilterChange('amountRequestedFrom', null)}
              />
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount requested to')}
                defaultValue={modalFilters?.amountRequestedTo}
                value={modalFilters?.amountRequestedTo}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountRequestedTo', values?.float)
                }}
                onClear={() => onFilterChange('amountRequestedTo', null)}
              />
            </div>
            <Divider height={'0.15rem'} />

            <div className="row">
              <DateInput
                size="m"
                className="full-width"
                dateFormat={'dd-MM-yyyy'}
                label={t('Processing started timestamp from')}
                value={modalFilters?.processingStartedDateFrom || ''}
                onChange={(value) => onFilterChange('processingStartedDateFrom', value)}
              />
              <DateInput
                size="m"
                className="full-width"
                dateFormat={'dd-MM-yyyy'}
                label={t('Processing started timestamp to')}
                value={modalFilters?.processingStartedDateTo || ''}
                onChange={(value) => onFilterChange('processingStartedDateTo', value)}
              />
            </div>
            <Divider />

            <div className="row">
              <DateInput
                size="m"
                className="full-width"
                dateFormat={'dd-MM-yyyy'}
                label={t('Processing ended timestamp from')}
                value={modalFilters?.processingEndedDateFrom || ''}
                onChange={(value) => onFilterChange('processingEndedDateFrom', value)}
              />
              <DateInput
                size="m"
                className="full-width"
                dateFormat={'dd-MM-yyyy'}
                label={t('Processing ended timestamp to')}
                value={modalFilters?.processingEndedDateTo || ''}
                onChange={(value) => onFilterChange('processingEndedDateTo', value)}
              />
            </div>
            <Divider />
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

export default TransactionLogs
