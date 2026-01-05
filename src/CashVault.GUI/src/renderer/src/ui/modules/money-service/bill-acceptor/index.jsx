/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Button from '@ui/components/button'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-top: 2.5rem;

  & .name {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.8125rem;
  }

  & .device-total-money {
    padding-top: 0.75rem;
    padding-bottom: 0.75rem;
    display: flex;
    color: black;
    font-weight: 500;
    font-size: 1.5rem;
    line-height: 2rem;
    border-bottom: 1px solid var(--primary-medium);

    & label {
      color: var(--primary-medium);
    }

    & span {
      margin-left: auto;
      display: flex;
      gap: 1rem;
      align-items: center;
      gap: 0.25rem;

      font-weight: 700;
      font-size: 2.125rem;

      & .currency {
        font-weight: 500;
        font-size: 2.125rem;
        line-height: 2.25rem;
      }
    }
  }

  & .actions {
    display: flex;
    gap: 1rem;
    align-items: center;
    padding-top: 0.75rem;
  }

  & .status {
    display: flex;
    gap: 1rem;
    align-items: center;

    font-weight: 500;
    font-size: 1.5rem;
    line-height: 1.875rem;

    & .active {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-weight: 600;
      color: var(--primary-dark);
    }

    & .enabled {
      display: flex;
      align-items: center;
      gap: 0.25rem;
    }
  }

  & .divider {
    width: 1px;
    height: 2rem;
    background-color: var(--primary-medium);
  }

  & .buttons {
    display: flex;
    gap: 0.75rem;
    align-items: center;
    margin-left: auto;
  }
`

const AcceptorItem = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 1rem;
  padding-bottom: 0.75rem;
  padding-top: 0.75rem;
  border-bottom: 1px solid var(--primary-medium);

  & .acceptor-item-title {
    font-size: 1rem;
    line-height: 1.75rem;
    text-transform: uppercase;
    font-weight: 600;
    color: var(--primary-medium);
  }
`

const BillAcceptor = ({
  totalMoney = 0,
  active = true,
  enabled = true,
  currency = 'KM',
  acceptorBillCount = 0,
  acceptorBillAmount = 0,
  acceptorTicketCount = 0,
  acceptorTicketAmount = 0,
  onEmptySave = () => {},
  onEnableDisable = () => {},
  isPending = false,
  notAvailable = false
}) => {
  const { t } = useTranslation()

  return (
    <Container>
      <div className="name">{t('Bill Acceptor')}</div>
      <div className="actions">
        <div className="status">
          <div className="active">
            <CheckboxInput label={t('Active')} value={active} size="s" />
          </div>

          <div className="divider" />

          <div className="enabled">
            <CheckboxInput
              label={enabled ? t('Enabled') : t('Disabled')}
              value={enabled}
              size="s"
            />
          </div>
        </div>

        <div className="buttons">
          <Button
            disabled={notAvailable}
            color={enabled ? 'dark' : 'light'}
            className="action-button"
            icon={(props) => {
              if (enabled) return <IconToggleOn {...props} />
              if (!enabled) return <IconToggleOff {...props} />
            }}
            onClick={() => onEnableDisable(enabled, DEVICE_TYPE.BILL_ACCEPTOR)}
          >
            {enabled && t('Enabled')}
            {!enabled && t('Disabled')}
          </Button>

          <Button disabled={isPending} onClick={onEmptySave} confirmAction={true}>
            {t('Empty')}
          </Button>
        </div>
      </div>
      <div className="form">
        <AcceptorItem>
          <DecimalInput
            label={t('Bills count')}
            disabled={true}
            size="m"
            value={acceptorBillCount}
            hasClear={false}
          />

          <DecimalInput
            label={t('Bills amount')}
            disabled={true}
            size="m"
            value={acceptorBillAmount}
            hasClear={false}
          />
        </AcceptorItem>

        <AcceptorItem>
          <DecimalInput
            label={t('Tickets count')}
            disabled={true}
            size="m"
            value={acceptorTicketCount}
            hasClear={false}
          />

          <DecimalInput
            label={t('Ticket amount')}
            disabled={true}
            size="m"
            value={acceptorTicketAmount}
            hasClear={false}
          />
        </AcceptorItem>
      </div>

      <div className="device-total-money">
        <label>{t('Total in Bill Acceptor')}:</label>

        <span>
          {totalMoney?.toFixed(2)}
          <span className="currency">{currency}</span>
        </span>
      </div>
    </Container>
  )
}

export default BillAcceptor
