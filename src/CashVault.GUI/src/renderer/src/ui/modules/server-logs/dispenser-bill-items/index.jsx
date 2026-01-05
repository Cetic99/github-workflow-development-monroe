import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Table from '@ui/components/table'
import BillDenominationSing from '@ui/components/bill-denomination-sign'
import IconMoneyOut from '@ui/components/icons/IconMoneyOut'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'

const Container = styled.div`
  & .cassette-number {
    font-weight: 600;
    font-size: 1.625rem;
    line-height: 1.875rem;
    padding-left: 1rem;
  }

  & .bill-denom {
    display: flex;
    flex-direction: row-reverse;
  }

  & .bill-count-requested {
    display: flex;
    flex-direction: row-reverse;
    gap: 0.5rem;
    align-items: center;

    font-weight: 600;
    font-size: 1.625rem;
    line-height: 1.875rem;
  }

  & .bill-count-dispensed {
    display: flex;
    flex-direction: row-reverse;
    gap: 0.5rem;
    align-items: center;
    padding-right: 1rem;

    font-weight: 600;
    font-size: 1.625rem;
    line-height: 1.875rem;
    color: var(--primary-medium);
  }

  & .dash {
    color: black;
  }

  & .rejected-count {
    color: var(--error-dark);
  }
`

const DispenserBillItems = ({ data = [] }) => {
  const { t } = useTranslation()

  const columns = [
    {
      id: 1,
      width: '10%',
      value: t('Cassette number'),
      render: (rowData) => {
        return (
          <div className="cell">
            <div className="cassette-number">{rowData?.cassetteNumber}</div>
          </div>
        )
      }
    },
    {
      id: 2,
      width: '30%',
      value: t('Bill denomination'),
      textAlign: 'end',
      render: (rowData) => {
        return (
          <div className="bill-denom cell">
            <BillDenominationSing value={rowData?.billDenomination} size="m" />
          </div>
        )
      }
    },
    {
      id: 1,
      width: '30%',
      value: t('Bill count requested'),
      textAlign: 'end',
      render: (rowData) => {
        return (
          <div className="bill-count-requested cell">
            <IconMoneyOut size="m" />
            <span>{rowData?.billCountRequested}</span>
          </div>
        )
      }
    },
    {
      id: 1,
      width: '30%',
      value: t('Bill count dispensed/rejected'),
      textAlign: 'end',
      render: (rowData) => {
        return (
          <div className="bill-count-dispensed">
            <IconBanknote01
              size="m"
              color={rowData?.billCountRejected > 0 ? 'var(--error-dark)' : 'var(--primary-medium)'}
            />

            {rowData?.billCountRejected > 0 && (
              <span className="rejected-count"> {rowData?.billCountRejected}</span>
            )}
            {rowData?.billCountRejected > 0 && <span className="dash">-</span>}

            <span>{rowData?.billCountDispensed}</span>
          </div>
        )
      }
    }
  ]

  return (
    <Container>
      <Table cellPadding="1rem 0.5rem" columns={columns} data={data || []} />
    </Container>
  )
}

export default DispenserBillItems
