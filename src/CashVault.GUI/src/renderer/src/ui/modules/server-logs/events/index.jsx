/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import Table from '@ui/components/table'
import { useTranslation } from '@domain/administration/stores'
import { useEffect, useRef, useState } from 'react'
import { useWindowSize } from 'react-use'
import { useEventLogs } from '@domain/records/queries'
import { getClosestPageSize } from '@src/app/services/utils'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'
import Pagination from '@ui/components/pagination'
import FullPageLoader from '@ui/components/full-page-loader'
import DateTimeCellRenderer from '@ui/components/cell-renders/date-time-cell-renderer'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'
import Divider from '@ui/components/divider'
import TextInput from '@ui/components/inputs/text-input'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import FilterButton from '@ui/components/filter-button'
import { EventLogType } from '@domain/records/constants'
import DateInput from '@ui/components/inputs/date-input'
import ItemsPerPage from '@ui/components/items-per-page'
import EventDetails from '../event-details'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'

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
  }

  & .footer {
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

const InitialFilters = {
  message: '',
  name: '',
  deviceType: '',
  eventType: '',
  timestampFrom: null,
  timestampTo: null
}

const EventLogs = () => {
  const { t } = useTranslation()
  const heightOffset = 250
  const rowHeight = 80

  //=======> Data

  const DeviceTypeOptions = [
    { value: '', name: t('None') },
    { value: DEVICE_TYPE.BILL_DISPENSER, name: t(DEVICE_TYPE.BILL_DISPENSER) },
    { value: DEVICE_TYPE.BILL_ACCEPTOR, name: t(DEVICE_TYPE.BILL_ACCEPTOR) },
    { value: DEVICE_TYPE.CARD_READER, name: t(DEVICE_TYPE.CARD_READER) },
    { value: DEVICE_TYPE.TITO_PRINTER, name: t(DEVICE_TYPE.TITO_PRINTER) }
  ]

  const EventTypeOptions = [
    { value: '', name: t('None') },
    { value: EventLogType.DEVICE_EVENT_LOG, name: t(EventLogType.DEVICE_EVENT_LOG) },
    { value: EventLogType.TRANSACTION_EVENT_LOG, name: t(EventLogType.TRANSACTION_EVENT_LOG) },
    { value: EventLogType.FAIL_EVENT_LOG, name: t(EventLogType.FAIL_EVENT_LOG) },
    { value: EventLogType.WARNING_EVENT_LOG, name: t(EventLogType.WARNING_EVENT_LOG) }
  ]

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

  const { data, isSuccess, isLoading } = useEventLogs({
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

  const onDetailsClick = (logItem) => {
    setLogDetails(logItem)
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
      width: null,
      value: t('timestamp'),
      render: (rowData) => {
        return <DateTimeCellRenderer value={rowData?.timestamp ?? ''} />
      }
    },
    { id: 2, width: null, value: t('Name'), accessor: 'name' },
    { id: 3, width: null, value: `${t('Type')} / ${t('Device Type')}`, accessor: 'type' },
    { id: 4, width: null, value: t('Message'), accessor: 'message' },
    {
      id: 5,
      width: '8%',
      value: t(''),
      render: (rowData) => (
        <div className="info-icon" onClick={() => onDetailsClick(rowData)}>
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
          //disabled={!isFilterApplied && !isLoading && data?.totalCount === 0}
          onClick={() => filterModalRef?.current?.showModal()}
          filters={filters}
        >
          {t('Filters')}
        </FilterButton>
      </div>

      <div className="table">
        <Table columns={columns} data={data?.items || []} zebra={true} />
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

      <EventDetails data={details} ref={modalDetailsRef} onClose={onCloseModalDetails} />

      {/* Filters modal */}
      <Modal ref={filterModalRef} onClose={() => filterModalRef?.current?.close()}>
        <ModalContent>
          <div className="header">{t('Filter Logs')}</div>
          <div className="content">
            <div className="row">
              <DropdownInput
                size="m"
                label={t('Type')}
                options={EventTypeOptions}
                value={modalFilters?.eventType ?? ''}
                onChange={(option) => onFilterChange('eventType', option?.value)}
              />
              <DropdownInput
                size="m"
                label={t('Device type')}
                options={DeviceTypeOptions}
                value={modalFilters?.deviceType ?? ''}
                onChange={(option) => onFilterChange('deviceType', option?.value)}
              />
            </div>
            <Divider />

            <div className="row">
              <TextInput
                size="m"
                className="full-width"
                label={t('Message')}
                value={modalFilters?.message ?? ''}
                placeholder={t('Enter keywords')}
                onChange={({ target }) => onFilterChange('message', target?.value)}
              />
              <TextInput
                size="m"
                className="full-width"
                label={t('Name')}
                value={modalFilters?.name ?? ''}
                placeholder={t('Enter name')}
                onChange={({ target }) => onFilterChange('name', target?.value)}
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
                dateFormat={'dd-MM-yyyy'}
                className="full-width"
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

export default EventLogs
