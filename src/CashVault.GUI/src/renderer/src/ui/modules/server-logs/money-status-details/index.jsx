/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import TextInput from '@ui/components/inputs/text-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { sanitizeTimestamp } from '@src/app/services/utils'
import Table from '@ui/components/table'
import Modal from '@ui/components/modal'
import { forwardRef } from 'react'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 3rem;

  & .title {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-top: 2rem;
  }

  & .details-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    padding-top: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--primary-medium);
  }

  & .details-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    tbody {
      text-align: center;
    }
  }
`

const Row = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 2rem;

  border-bottom: 1px solid var(--primary-medium);
  padding-bottom: 1rem;
`

const MoneyStatusDetails = forwardRef(({ data = {}, onClose }, ref) => {
  const { t } = useTranslation()

  const transformForTable = (data) => {
    const rows = []

    for (const key in data) {
      const match = key.match(/^(old|new)([A-Z][a-zA-Z]+)$/)
      if (!match) continue

      const type = match[2]
      const label = type.replace(/([A-Z])/g, ' $1').trim()

      const existing = rows.find((row) => row.label === label)
      if (!existing) {
        rows.push({
          label: t(label),
          oldValue: key.startsWith('old') ? data[key] : null,
          newValue: key.startsWith('new') ? data[key] : null
        })
      } else {
        if (key.startsWith('old')) existing.oldValue = data[key]
        if (key.startsWith('new')) existing.newValue = data[key]
      }
    }

    return rows
  }

  const columns = [
    {
      id: 1,
      width: null,
      value: t('Cassette number'),
      accessor: 'cassetteNumber'
    },
    {
      id: 2,
      width: null,
      value: t('Bill denomination'),
      accessor: 'billDenomination'
    },
    {
      id: 3,
      width: null,
      value: t('old bill count'),
      accessor: 'oldBillCount'
    },
    {
      id: 4,
      width: null,
      value: t('new bill count'),
      accessor: 'newBillCount'
    }
  ]

  const acceptorColumns = [
    {
      id: 1,
      width: null,
      value: t(''),
      accessor: 'label'
    },
    {
      id: 2,
      width: null,
      value: t('New value'),
      accessor: 'newValue'
    },
    {
      id: 3,
      width: null,
      value: t('Old value'),
      accessor: 'oldValue'
    }
  ]

  return (
    <Modal ref={ref} hasClose={true} size="l" onClose={onClose}>
      <Container>
        <div className="details-header">{t('Money status details')}</div>

        <div className="details-content">
          <TextInput
            size="m"
            label={t('Device Type')}
            value={data?.deviceType || ''}
            disabled={true}
          />
          <Row>
            <TextInput size="m" label={t('Stats')} value={data?.status ?? ''} disabled={true} />
            <TextInput size="m" label={t('Type')} value={data?.type ?? ''} disabled={true} />
          </Row>
          <Row>
            <DecimalInput
              size="m"
              label={t('Amount')}
              value={data?.amount ?? ''}
              sufix={data?.currency?.symbol}
              disabled={true}
              hasClearButton={false}
            />
            <TextInput
              size="m"
              label={t('Time')}
              value={sanitizeTimestamp(data?.timestamp) ?? ''}
              disabled={true}
            />
          </Row>

          <Row>
            <DecimalInput
              size="m"
              label={t('Old device amount')}
              value={data?.oldDeviceBillAmount ?? ''}
              sufix={data?.currency?.symbol}
              disabled={true}
              hasClearButton={false}
            />
            <DecimalInput
              size="m"
              label={t('New device amount')}
              value={data?.newDeviceBillAmount ?? ''}
              sufix={data?.currency?.symbol}
              disabled={true}
              hasClearButton={false}
            />
          </Row>

          {data?.deviceType == DEVICE_TYPE.BILL_DISPENSER && (
            <>
              <div className="title"> {t('Bill dispenser')}</div>
              <Table
                columns={columns}
                data={data?.dispenserMoneyStatus || []}
                zebra={false}
                noDataText={t('No data')}
              />
            </>
          )}

          {data?.deviceType == DEVICE_TYPE.BILL_ACCEPTOR && (
            <>
              <div className="title"> {t('Bill acceptor')}</div>
              <Table
                columns={acceptorColumns}
                data={transformForTable(data?.acceptorMoneyStatus) || []}
                zebra={false}
                noDataText={t('No data')}
              />
            </>
          )}
        </div>
      </Container>
    </Modal>
  )
})

export default MoneyStatusDetails
