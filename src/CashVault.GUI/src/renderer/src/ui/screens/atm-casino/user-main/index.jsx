/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import TextDivider from '@ui/components/text-divider'
import CurrentBalanceV1 from '../../../components/current-balance-v1'
import { useEffect } from 'react'
import { useCreditsAmount } from '@domain/transactions/store'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/transactions/commands'
import DeviceWarnings from '@ui/components/device-warnings'
import { useTranslation } from '@domain/administration/stores'
import {
  useShowBillDispenserWarning,
  useShowBillAcceptorWarning,
  useShowTITOPrinterWarning
} from '@domain/device/stores'
import WidgetsContainer from '@ui/components/widgets'
import { useUserWidgets } from '@src/app/domain/configuration/stores'

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .current-credit {
    margin-top: auto;
    margin-bottom: 1rem;
  }
`

const UserMainScreen = () => {
  const { t } = useTranslation()

  const creditsAmount = useCreditsAmount()
  const showPrinterWarning = useShowTITOPrinterWarning()
  const showBillDispenserWarning = useShowBillDispenserWarning()
  const showBillAcceptorWarning = useShowBillAcceptorWarning()

  const widgets = useUserWidgets()

  useEffect(() => {
    Mediator.dispatch(CommandType.GetCreditsAmount, {})
  }, [])

  return (
    <ScreenContainer hasBackgroundImage={true}>
      <Container>
        <DeviceWarnings
          showPrinterWarning={showPrinterWarning}
          showBillDispenserWarning={showBillDispenserWarning}
          showBillAcceptorWarning={showBillAcceptorWarning}
        />
        <div className="current-credit">
          <CurrentBalanceV1
            value={creditsAmount.amount}
            currency={creditsAmount?.currencySymbol}
            precision={creditsAmount?.amountPrecision}
          />
        </div>
        <TextDivider>{t('Please choose your action')}</TextDivider>

        <WidgetsContainer widgets={widgets} />
      </Container>
    </ScreenContainer>
  )
}

export default UserMainScreen
