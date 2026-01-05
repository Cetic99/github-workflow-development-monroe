import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import CardButton from '@ui/components/card-button'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconReceipt from '@ui/components/icons/IconReceipt'
import IconPrinter from '@ui/components/icons/IconPrinter'
import IconMonitor from '@ui/components/icons/IconMonitor'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
    margin-bottom: 3rem;
    h3 {
      font-size: 1rem;
      text-transform: uppercase;
      color: var(--secondary-dark);
      margin-top: 1.5rem;
    }

    h1 {
      font-size: 4.375rem;
      font-weight: 800;
      line-height: 4.375rem;
      margin-top: 1rem;
    }

    h2 {
      font-size: 1.625rem;
      color: var(--primary-dark);
      font-weight: 400;
      margin-top: 1.5rem;
    }
  }
  & .footer {
    padding-top: 3rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;
    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }

  & .cards {
    display: flex;
    gap: 1.25rem;
    width: 100%;

    & > * {
      width: 17.188rem;
    }
  }
`

const CheckBalanceScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/')
  }

  const handleReceiptPrintValue = (value) => {
    navigate(`/atm/loading?to=${value ? 'balance' : 'take-receipt'}`)
  }

  return (
    <ScreenContainer
      hasLogo={true}
      hasOnGoBack={false}
      hasExitButton={false}
      hasMasterAuthButton={false}
      hasUserButton={false}
      hasLanguageSwitcher={false}
      hasSettingsButton={true}
      urlPrefix="atm"
    >
      <Container>
        <div className="header">
          <IconReceipt size="xl" color="var(--secondary-dark)" />

          <h3>Check balance</h3>
          <h1>
            Display on screen or
            <br /> print receipt?
          </h1>
          <h2>Please select.</h2>
        </div>

        <div className="cards">
          <CardButton
            className="last-withdrawal"
            text={t('On screen')}
            icon={<IconMonitor size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleReceiptPrintValue(true)}
          />
          <CardButton
            className="last-withdrawal"
            text={t('Receipt')}
            icon={<IconPrinter size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleReceiptPrintValue(false)}
          />
        </div>

        <div className="footer">
          <CircleButton
            icon={(props) => <IconLeftHalfArrow {...props} />}
            size="l"
            color="medium"
            textRight={t('Back')}
            onClick={onGoBack}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default CheckBalanceScreen
