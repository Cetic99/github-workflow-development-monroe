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
  useMoneyStatusTransactionLogs,
  getMoneyStatusTransactionLogDetailsData
} from '@domain/records/queries'
import AmountCellRender from '@ui/components/cell-renders/amount-cell-renderer'
import MoneyStatusTransactionTypeCellRenderer from '@ui/components/cell-renders/money-status-transaction-type-renderer'
import { TransactionStatus } from '@domain/transactions/contants'
import { DEVICE_TYPE } from '@domain/device/constants'

import FilterButton from '@ui/components/filter-button'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'
import Divider from '@ui/components/divider'
import DateInput from '@ui/components/inputs/date-input'
import ItemsPerPage from '@ui/components/items-per-page'
import MoneyStatusDetails from '../money-status-details'

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
  timestampFrom: null,
  timestampTo: null,
  amountFrom: null,
  amountTo: null,
  typeId: null,
  kindId: null
}

const MoneyStatusLogs = () => {
  const { t } = useTranslation()
  const heightOffset = 250
  const rowHeight = 80

  //=======> State

  const windowSize = useWindowSize()

  const filterModalRef = useRef()
  const modalDetailsRef = useRef()

  const [filters, setFilters] = useState({})
  const [totalRowCount, setTotalRowCount] = useState(0)
  const [details, setLogDetails] = useState(null)
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10
  })

  const [modalFilters, setModalFilters] = useState(InitialFilters)

  //=======> Queries

  const { data, isSuccess, isLoading } = useMoneyStatusTransactionLogs({
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
    var details = await getMoneyStatusTransactionLogDetailsData({ id: ticketId })
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
      width: '20%',
      value: t('Time'),
      render: (rowData) => {
        return <DateTimeCellRenderer value={rowData?.timestamp} />
      }
    },
    {
      id: 2,
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
      id: 6,
      value: `${t('Type')} / ${t('Kind')}`,
      width: '25%',
      render: (rowData) => {
        var type = rowData?.type?.code?.toLowerCase()

        return <MoneyStatusTransactionTypeCellRenderer type={type} />
      }
    },
    {
      id: 4,
      value: t('Device Type'),
      width: '35%',
      render: (rowData) => {
        var status = rowData?.status?.code?.toLowerCase()
        //TODO: Reformat this to use deviceType from rowData
        const deviceType =
          rowData?.deviceType === DEVICE_TYPE.BILL_DISPENSER ? 'Bill dispenser' : 'Bill acceptor'

        return <div>{t(deviceType)}</div>
      }
    },

    {
      id: 5,
      width: '8%',
      value: t(''),
      render: (rowData) => (
        <div className="info-icon" onClick={() => onDetailsClick(rowData?.id)}>
          <IconInfoSquare />
        </div>
      )
    }
  ]

  const rowColor = []

  data?.transactions?.forEach((x, i) => {
    if (x.status.code.toLowerCase() === TransactionStatus.FAILED) {
      rowColor.push({
        index: i,
        color: '#FFDCDC'
      })
    }

    if (x.status.code.toLowerCase() === TransactionStatus.PARTIALLY_COMPLETED) {
      rowColor.push({
        index: i,
        color: '#F4E0CA'
      })
    }
  })

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
        <Table columns={columns} data={data?.transactions || []} rowColor={rowColor} />
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

      <MoneyStatusDetails ref={modalDetailsRef} data={details} onClose={onCloseModalDetails} />

      {/* Filters modal */}
      <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
        <ModalContent>
          <div className="header">{t('Filter Logs')}</div>
          <div className="content">
            <div className="row">
              <DropdownInput
                size="m"
                label={t('Type')}
                options={[
                  { id: null, code: t('None') },
                  ...(data?.moneyStatusTransactionTypes || [])
                ]}
                value={modalFilters?.typeId ?? ''}
                optionLabelName="code"
                optionValueName="id"
                onChange={(option) => onFilterChange('typeId', option.id)}
                className="full-width"
              />
            </div>

            <Divider />

            <div className="row">
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount from')}
                value={modalFilters?.amountFrom ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountFrom', values?.float)
                }}
                onClear={() => onFilterChange('amountFrom', null)}
                className="full-width"
              />
              <DecimalInput
                size="m"
                allowDecimals={false}
                allowNegativeValue={false}
                label={t('Amount to')}
                value={modalFilters?.amountTo ?? ''}
                onValueChange={(value, name, values) => {
                  onFilterChange('amountTo', values?.float)
                }}
                onClear={() => onFilterChange('amountTo', null)}
                className="full-width"
              />
            </div>
            <Divider />

            <div className="row">
              <DateInput
                size="m"
                label={t('From')}
                value={modalFilters?.timestampFrom ?? ''}
                dateFormat={'dd-MM-yyyy'}
                className="full-width"
                onChange={(value) => onFilterChange('timestampFrom', value)}
              />
              <DateInput
                size="m"
                label={t('To')}
                value={modalFilters?.timestampTo ?? ''}
                className="full-width"
                dateFormat={'dd-MM-yyyy'}
                onChange={(value) => onFilterChange('timestampTo', value)}
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

export default MoneyStatusLogs
