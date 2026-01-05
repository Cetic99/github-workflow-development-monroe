/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import Breadcrumbs from '@ui/components/breadcrumbs'
import BillAcceptor from '@ui/modules/money-service/bill-acceptor'
import BillDispenser from '@ui/modules/money-service/bill-dispenser'
import MoneyDevicesMaintenance from '@ui/modules/money-service/money-devices-maintenance'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import useDevices from '@domain/administration/hooks/use-devices'
import { useMemo } from 'react'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'
import { useGetDeviceByType } from '@domain/device/stores'
import { useMoneyService } from '@domain/reports/queries'
import FullPageLoader from '@ui/components/full-page-loader'
import { useSaveMoneyBillDispenserRefillData } from '@domain/configuration/mutations/money'
import { useEmptyBillTicketAcceptor } from '@domain/configuration/mutations/money'
import { useEmptyBillDispenserCassettes } from '@domain/configuration/mutations/money'
import { useNavigate } from 'react-router-dom'
import Alert from '@ui/components/alert'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .total-money {
    padding-top: 1.5rem;
    display: flex;
    align-items: center;
    gap: 1rem;

    & label {
      color: black;
      font-weight: 500;
      font-size: 1.5rem;
      line-height: 1.625rem;
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

  & .money-service-header {
  }

  & .content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    overflow: auto;
    height: 100%;
    padding-right: 1rem;
  }

  & .back-action {
    margin-top: auto;
    padding: 0.5rem 0 0 0;
    position: sticky;
    bottom: 0;
    z-index: 10;
    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const MoneyServiceScreen = () => {
  const { t } = useTranslation()
  const { onEnableDisable, isLoading } = useDevices()
  const navigate = useNavigate()

  const billdispenser = useGetDeviceByType(DEVICE_TYPE.BILL_DISPENSER)
  const billAcceptor = useGetDeviceByType(DEVICE_TYPE.BILL_ACCEPTOR)
  const { data: moneyStatus, refetch } = useMoneyService()

  const preparedData = useMemo(() => {
    let tempData = []

    if (billdispenser) {
      tempData.push({
        name: 'Bill dispenser',
        type: DEVICE_TYPE.BILL_DISPENSER,
        active: billdispenser?.isActive || false,
        enabled: billdispenser?.isEnabled || false,
        onEnableDisable: onEnableDisable
      })
    }

    if (billAcceptor) {
      tempData.push({
        name: 'Bill acceptor',
        type: DEVICE_TYPE.BILL_ACCEPTOR,
        active: billAcceptor?.isActive || false,
        enabled: billAcceptor?.isEnabled || false,
        onEnableDisable: onEnableDisable
      })
    }

    return tempData
  }, [billdispenser, billAcceptor, onEnableDisable])

  //===========================================================================

  const { mutate: refillCommand, isPending: isPendingRefillDispenser } =
    useSaveMoneyBillDispenserRefillData(
      () => refetch(),
      () => {}
    )

  const onDispenserRefillSave = (cassettes) => {
    refillCommand(cassettes)
  }

  const { mutate: emptyCassettesCommand, isPending: isPendingEmptyDispenser } =
    useEmptyBillDispenserCassettes(
      () => refetch(),
      () => {}
    )

  const { mutate: emptyAcceptorCommand, isPending: isPendingEmptyAcceptor } =
    useEmptyBillTicketAcceptor(() => refetch())

  const onEmptyDispenserSave = (cassettesNumbers, emptyRejectBin) => {
    emptyCassettesCommand(cassettesNumbers, emptyRejectBin)
  }

  const onEmptyAcceptorSave = () => {
    emptyAcceptorCommand()
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <FullPageLoader
        loading={
          isLoading || isPendingRefillDispenser || isPendingEmptyDispenser || isPendingEmptyAcceptor
        }
      />
      <Container>
        <div className="money-service-header">
          <Breadcrumbs names={[t('Money service')]} />
        </div>

        <div className="content">
          {(billdispenser || billAcceptor) && (
            <div className="total-money">
              <label>{t('Total money in devices')}:</label>

              <span>
                {moneyStatus?.totalAmount || 0}
                <span className="currency">{moneyStatus?.currencySymbol}</span>
              </span>
            </div>
          )}

          <MoneyDevicesMaintenance devices={preparedData} />
          {!billdispenser && (
            <Alert severity="warning" text={t('Bill dispenser device not available')} />
          )}
          <BillDispenser
            notAvailable={!billdispenser}
            enabled={billdispenser?.isEnabled || false}
            active={billdispenser?.isActive || false}
            onEnableDisable={onEnableDisable}
            cassettes={moneyStatus?.dispenserCassettes || []}
            currency={moneyStatus?.currencySymbol || 'KM'}
            totalMoney={moneyStatus?.dispenserTotalAmount || 0}
            onRefillSave={onDispenserRefillSave}
            onEmptySave={onEmptyDispenserSave}
            isPending={isPendingRefillDispenser || isPendingEmptyDispenser}
            dispenserRejectedBillsCount={moneyStatus?.dispenserRejectedBillsCount || 0}
          />

          {!billAcceptor && (
            <Alert severity="warning" text={t('Bill acceptor device not available')} />
          )}
          <BillAcceptor
            notAvailable={!billAcceptor}
            enabled={billAcceptor?.isEnabled || false}
            active={billAcceptor?.isActive || false}
            currency={moneyStatus?.currencySymbol || 'KM'}
            acceptorBillCount={moneyStatus?.acceptorBillCount || 0}
            acceptorBillAmount={moneyStatus?.acceptorBillAmount || 0}
            acceptorTicketCount={moneyStatus?.acceptorTicketCount || 0}
            acceptorTicketAmount={moneyStatus?.acceptorTicketAmount || 0}
            totalMoney={moneyStatus?.acceptorTotalAmount || 0}
            onEnableDisable={onEnableDisable}
            onEmptySave={onEmptyAcceptorSave}
            isPending={isPendingEmptyAcceptor}
          />
          <div className="back-action">
            <CircleButton
              size="l"
              textRight={t('Back')}
              icon={(props) => <IconLeftHalfArrow {...props} />}
              onClick={() => navigate('/')}
              shadow={true}
            />
          </div>
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default MoneyServiceScreen
