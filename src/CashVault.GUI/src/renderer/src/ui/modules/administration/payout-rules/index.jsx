/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { usePayoutRules } from '@domain/administration/queries'
import { useSavePayoutRules } from '@domain/administration/commands'
import { useTranslation } from '@domain/administration/stores'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Divider from '@ui/components/divider'
import { useEffect, useState } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'
import FullPageLoader from '@ui/components/full-page-loader'

const Container = styled.div`
  display: flex;
  height: 100%;
  flex-direction: column;
  gap: 0.75rem;
  padding-top: 0.5rem;

  & .main-title {
    font-size: 1.5rem;
  }
  & .title {
    font-size: 1.25rem;
    font-weight: 700;
  }

  & .tab {
    margin: 0.5rem 0 1rem 0;
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  & .row {
    display: flex;
    align-items: center;
    gap: 1rem;

    > div {
      width: 100%;
    }
  }

  & .row-title {
    font-family: Poppins;
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    letter-spacing: -2%;
    color: black;

    padding: 0.5rem 0;
  }

  & .pts-footer {
    margin-top: auto;
    display: flex;
    gap: 4rem;
    width: 100%;
    justify-content: space-between;
  }
`

const PayoutRules = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const { data, isLoading, isSuccess } = usePayoutRules()
  const command = useSavePayoutRules()

  useEffect(() => {
    setPayoutRules(data)
  }, [data])

  const [payoutRules, setPayoutRules] = useState(data)

  const onSave = () => {
    const payloadData = {
      ticketsMin: payoutRules.tickets?.min || 0,
      ticketsMax: payoutRules.tickets?.max || 0,
      billsMin: payoutRules.bills?.min || 0,
      billsMax: payoutRules.bills?.max || 0,
      coinsMin: payoutRules.coins?.min || 0,
      coinsMax: payoutRules.coins?.max || 0
    }

    command.mutate(payloadData)
  }

  const onInputChange = (dataAccessor, valueAccessor, value) => {
    setPayoutRules((prev) => {
      const state = { ...prev }
      state[dataAccessor][valueAccessor] = value

      return state
    })
  }

  return (
    <Container>
      <FullPageLoader loading={isLoading | command.isPending} />

      <form id="payout-rules">
        {isSuccess && (
          <>
            <div className="tab">
              <div className="row-title">{t('Tickets')}</div>
              <div className="row">
                <DecimalInput
                  size="m"
                  name="ticketsMin"
                  label={`${t('Amount')} Min`}
                  inputType="number"
                  defaultValue={payoutRules?.tickets?.min}
                  value={payoutRules?.tickets?.min}
                  info={payoutRules?.tickets?.info}
                  onValueChange={(value, name, values) => {
                    onInputChange('tickets', 'min', value)
                  }}
                  onClear={() => {
                    onInputChange('tickets', 'min', 0)
                  }}
                />
                <DecimalInput
                  size="m"
                  name="ticketsMax"
                  inputType="number"
                  label={`${t('Amount')} Max`}
                  defaultValue={payoutRules?.tickets?.max}
                  value={payoutRules?.tickets?.max}
                  info={payoutRules?.tickets?.info}
                  onValueChange={(value, name, values) => {
                    onInputChange('tickets', 'max', values.float)
                  }}
                  onClear={() => {
                    onInputChange('tickets', 'max', 0)
                  }}
                />
              </div>
            </div>

            <Divider height={'0.15rem'} />

            <div className="tab">
              <div className="row-title">{t('Bills')}</div>
              <div className="row">
                <DecimalInput
                  size="m"
                  name="billsMin"
                  label={`${t('Amount')} Min`}
                  value={payoutRules?.bills?.min}
                  info={payoutRules?.bills?.info}
                  decimalLimit={0}
                  onValueChange={(value, name, values) => {
                    onInputChange('bills', 'min', values?.float)
                  }}
                  onClear={() => {
                    onInputChange('bills', 'min', 0)
                  }}
                />
                <DecimalInput
                  size="m"
                  name="billsMax"
                  label={`${t('Amount')} Max`}
                  value={payoutRules?.bills?.max}
                  info={payoutRules?.bills?.info}
                  decimalLimit={0}
                  onValueChange={(value, name, values) => {
                    onInputChange('bills', 'max', values?.float)
                  }}
                  onClear={() => {
                    onInputChange('bills', 'max', 0)
                  }}
                />
              </div>
            </div>

            <Divider height={'0.15rem'} />

            {/* <div className="tab">
            <div className="row-title">{t('Coins')}</div>
              <div className="row">
                <DecimalInput
                  size='m'
                  name="coinsMin"
                  label={`${t('Amount')} Min`}
                  value={payoutRules?.coins?.min}
                  info={payoutRules?.coins?.info}
                  onValueChange={(value, name, values) => {
                    onInputChange('coins', 'min', value)
                  }}
                  onClear={() => {
                    onInputChange('coins', 'min', 0)
                  }}
                />
                <DecimalInput
                  size='m'
                  name="coinsMax"
                  label={`${t('Amount')} Max`}
                  value={payoutRules?.coins?.max}
                  info={payoutRules?.coins?.info}
                  onValueChange={(value, name, values) => {
                    onInputChange('coins', 'max', value)
                  }}
                  onClear={() => {
                    onInputChange('coins', 'max', 0)
                  }}
                />
              </div>
            </div>

            <Divider height={'0.15rem'} /> */}
          </>
        )}
      </form>

      <div className="pts-footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={() => navigate(-1)}
          shadow={true}
        />

        <CircleButton
          icon={(props) => <IconRightHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Accept')}
          onClick={onSave}
          disabled={false}
          shadow={true}
        />
      </div>
    </Container>
  )
}

export default PayoutRules
