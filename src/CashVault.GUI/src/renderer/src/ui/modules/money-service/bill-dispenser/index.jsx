/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Button from '@ui/components/button'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import { useRef } from 'react'
import RefillDispenserDialog from '../refill-dispenser-dialog'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import EmptyDispenserDialog from '../empty-dispenser-dialog'
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
    display: flex;
    color: black;
    font-weight: 500;
    font-size: 1.5rem;
    line-height: 2rem;

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

const Casssette = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 1rem;
  padding-bottom: 0.75rem;
  padding-top: 0.75rem;
  border-bottom: 1px solid var(--primary-medium);
`

const RejectBin = styled.div`
  text-align: end;
  display: flex;
  align-items: center;
  font-weight: 500;
  font-size: 1.5rem;
  line-height: 1.875rem;
  width: 100%;
  justify-content: flex-end;
`

const BillDispenser = ({
  totalMoney = 0,
  active = true,
  enabled = true,
  currency = 'BAM',
  cassettes = [],
  dispenserRejectedBillsCount = 0,
  onRefillSave = () => {},
  onEmptySave = () => {},
  onEnableDisable = () => {},
  isPending = false,
  notAvailable = false
}) => {
  const { t } = useTranslation()

  const refillModalRef = useRef()
  const emptyModalRef = useRef()

  const onRefillClick = () => {
    refillModalRef?.current?.showModal()
  }

  const onCloseRefill = () => {
    refillModalRef?.current?.close()
  }

  const onEmptyClick = () => {
    emptyModalRef?.current?.showModal()
  }

  const onCloseEmpty = () => {
    emptyModalRef?.current?.close()
  }

  return (
    <Container>
      <RefillDispenserDialog
        key={`refill-dispenser-dialog-${cassettes?.reduce((sum, x) => sum + (x?.billCount || 0), 0)}`}
        ref={refillModalRef}
        onClose={onCloseRefill}
        cassettes={cassettes}
        onSave={onRefillSave}
      />
      <EmptyDispenserDialog
        key={`empty-dispenser-dialog-${cassettes?.reduce((sum, x) => sum + (x?.billCount || 0), 0)}`}
        ref={emptyModalRef}
        cassettes={cassettes}
        dispenserRejectedBillsCount={dispenserRejectedBillsCount}
        onClose={onCloseEmpty}
        onSave={onEmptySave}
      />

      <div className="name">{t('Bill Dispenser')}</div>

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
            onClick={() => onEnableDisable(enabled, DEVICE_TYPE.BILL_DISPENSER)}
          >
            {enabled && t('Enabled')}
            {!enabled && t('Disabled')}
          </Button>

          <Button disabled={isPending} onClick={onRefillClick}>
            {t('Refill')}
          </Button>

          <Button disabled={isPending} onClick={onEmptyClick}>
            {t('Empty')}
          </Button>
        </div>
      </div>

      <div className="form">
        {cassettes
          ?.sort((a, b) => a?.cassetteNumber - b?.cassetteNumber)
          ?.map((x, i) => (
            <Casssette key={`${x?.name}__${i}`}>
              <div>
                <DecimalInput
                  disabled={true}
                  label={x.name}
                  size="m"
                  value={x.billCount}
                  hasClear={false}
                />
              </div>

              <div>
                <DecimalInput
                  disabled={true}
                  label={t('Amount')}
                  size="m"
                  value={x.totalAmount}
                  hasClear={false}
                />
              </div>
            </Casssette>
          ))}

        <Casssette>
          <RejectBin>{t('Rejected bills')}</RejectBin>
          <div>
            <DecimalInput
              disabled={true}
              label={t('Bills count')}
              size="m"
              value={dispenserRejectedBillsCount}
              hasClear={false}
            />
          </div>
        </Casssette>
      </div>

      <div className="device-total-money">
        <label>{t('Total in Bill Dispenser')}:</label>

        <span>
          {totalMoney}
          <span className="currency">{currency}</span>
        </span>
      </div>
    </Container>
  )
}

export default BillDispenser
