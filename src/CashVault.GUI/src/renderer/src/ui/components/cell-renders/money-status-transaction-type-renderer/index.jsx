import styled from '@emotion/styled'
import { MoneyStatusTransactionType } from '@domain/transactions/contants'
import IconArrowUp from '@ui/components/icons/IconArrowUp'
import IconArrowDown from '@ui/components/icons/IconArrowDown'
import { useTranslation } from '@domain/administration/stores'

const Cell = styled.div`
  display: flex;
  gap: 0.25rem;
  text-align: right;
  align-items: center;

  color: var(--primary-medium);

  & .amount-cell-content {
    display: flex;
    gap: 0.25rem;
    flex-direction: column;
    flex-grow: 10;

    & .amount {
      font-weight: 700;
      font-size: 1.5rem;
      line-height: 1.75rem;
    }
  }

  & .amount-cell-icon {
    flex-shrink: 0;
  }
`

const MoneyStatusTransactionTypeCellRenderer = ({ type }) => {
  const { t } = useTranslation()

  return (
    <Cell type={type}>
      <div className="amount-cell-content">
        <div className="amount">{t(type)}</div>
      </div>

      <div className="amount-cell-icon">
        {type?.toLowerCase() === MoneyStatusTransactionType.REFILL.toLowerCase() && (
          <IconArrowUp color="var(--primary-dark)" size="m" />
        )}
        {type?.toLowerCase() === MoneyStatusTransactionType.HARVEST.toLowerCase() && (
          <IconArrowDown color="var(--error-dark)" size="m" />
        )}
      </div>
    </Cell>
  )
}

export default MoneyStatusTransactionTypeCellRenderer
