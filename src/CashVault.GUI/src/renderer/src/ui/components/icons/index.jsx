/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import Table from '@ui/components/table'
import { useTranslation } from '@domain/administration/stores'
import { TransactionStatus } from '@domain/transactions/contants'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'
import IconCheckCircle from '@ui/components/icons/IconCheckCircle'

const Container = styled.div``

const TwoRowCell = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.25rem;

  & .date-time-cell {
    font-weight: 500;
    font-size: 1.125rem;
    line-height: 1.5rem;
  }
`

const OpenDetailsButton = styled.div``

const TransactionStatusCell = styled.div`
  display: flex;
  gap: 0.25rem;
  align-items: center;

  & .cell-icon {
    flex-shrink: 0;
  }

  & .cell-content {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    font-weight: 500;
    font-size: 1.125rem;
    line-height: 1.5rem;
  }

  & .blue {
    color: var(--primary-light);
  }

  & .green {
    color: var(--primary-medium);
  }

  & .orange {
    color: orange;
  }

  & .red {
    color: var(--error-dark);
  }
`

const AmountCell = styled.div`
  ${(p) => {
    if (p.status === 'pending') return `background-color: ;`
    if (p.status === 'completed') return `color: ;`
    if (p.status === 'partially_completed') return `color: ;`
    if (p.status === 'failed') return `color: ;`
  }}
`

const TransactionStatusCellRender = ({ status, description, requestedAmount }) => {
  const { t } = useTranslation()

  return (
    <TransactionStatusCell status={status}>
      <div className="cell-icon">
        {status === TransactionStatus.PENDING && <IconCheckCircle size="m" color="blue" />}
        {status === TransactionStatus.COMPLETED && (
          <IconCheckCircle size="m" color="var(--primary-dark)" />
        )}
        {status === TransactionStatus.PARTIALLY_COMPLETED && (
          <IconExclamationMarkCircle size="m" color="orange" />
        )}
        {status === TransactionStatus.FAILED && (
          <IconExclamationMarkCircle size="m" color="var(--error-dark)" />
        )}
      </div>

      <div className="cell-content">
        {status === TransactionStatus.PENDING && <div className="status blue">{t('Pending')}</div>}
        {status === TransactionStatus.COMPLETED && (
          <div className="status green">{t('Completed')}</div>
        )}
        {status === TransactionStatus.PARTIALLY_COMPLETED && (
          <div className="status orange">{t('Partially completed')}</div>
        )}
        {status === TransactionStatus.FAILED && <div className="status red">{t('Failed')}</div>}

        {description && <div className="description">{description}</div>}

        {requestedAmount && (
          <div>
            {t('Requested')}: {requestedAmount}
          </div>
        )}
      </div>
    </TransactionStatusCell>
  )
}

const AmountCellRender = ({ amount, currency }) => {
  return (
    <AmountCell status={status}>
      <div className="amount-cell-content">
        <div className="amount">{amount}</div>
        <div className="currency">{currency?.symbol}</div>
      </div>

      <div className="amount-cell-icon"></div>
    </AmountCell>
  )
}

const TransactionLogs = () => {
  const { t } = useTranslation()

  const columns = [
    {
      id: 1,
      value: t('Time'),
      width: '20%',
      render: (rowData) => {
        return (
          <TwoRowCell className="date-time-cell">
            <span>{rowData?.date}</span>
            <span>{rowData?.time}</span>
          </TwoRowCell>
        )
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
            amount={rowData?.amount}
            currency={rowData?.currency}
          />
        )
      }
    },
    {
      id: 3,
      value: t('Status'),
      width: '35%',
      render: (rowData) => {
        var status = rowData?.status?.code?.toLowerCase()

        return <TransactionStatusCellRender status={status} description={'Bill dispenser'} />
      }
    },
    {
      id: 4,
      value: t('Is CMS'),
      width: '10%',
      render: () => {
        return <CheckboxInput />
      }
    },
    {
      id: 5,
      width: '10%',
      render: () => {
        return <OpenDetailsButton></OpenDetailsButton>
      }
    }
  ]

  const data = [
    {
      id: 1,
      date: '01.01.2025',
      time: '22:22.2222',
      amount: 1200.2,
      currency: { symbol: 'BAM', position: 'position' },
      isCms: true,
      type: {
        code: 'Credit'
      },
      status: {
        code: 'Completed'
      }
    },
    {
      id: 2,
      date: '01.01.2025',
      time: '22:22.2222',
      amount: 1200.2,
      currency: { symbol: 'BAM', position: 'position' },
      isCms: true,
      type: {
        code: 'Credit'
      },
      status: {
        code: 'partiallycompleted'
      }
    },
    {
      id: 2,
      date: '01.01.2025',
      time: '22:22.2222',
      amount: 1200.2,
      currency: { symbol: 'BAM', position: 'position' },
      isCms: true,
      type: {
        code: 'Debit'
      },
      status: {
        code: 'failed'
      }
    }
  ]

  const rowColor = []

  data?.forEach((x, i) => {
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

  return (
    <Container>
      <Table columns={columns} data={data} rowColor={rowColor} />
    </Container>
  )
}

export default TransactionLogs
