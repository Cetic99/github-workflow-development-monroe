import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconQrCode from '@ui/components/icons/IconQrCode'
import styled from '@emotion/styled'
import ScanMobile from '@ui/assets/images/atm/scan-mobile.svg'
import IconCalculator from '@ui/components/icons/IconCalculator'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
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

  & .content {
    position: relative;
    height: 15rem;
    & .scan-mobile {
      top: -5rem;
      left: -3rem;
      position: absolute;
      width: 35rem;
    }
  }
`

const CardlessTransactionScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/')
  }
  const handleOnProceed = () => {
    navigate('/atm/cardless-transaction/code')
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
          <IconQrCode size="xl" color="var(--secondary-dark)" />

          <h3>Cardless Transaction</h3>
          <h1>
            Please scan the
            <br /> QR code
          </h1>
          <h2>Place your mobile phone under the scanner.</h2>
        </div>

        <div className="content">
          <img src={ScanMobile} alt="Mobile scan" className="scan-mobile" />
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
          <CircleButton
            icon={(props) => <IconCalculator {...props} />}
            size="l"
            color="dark"
            textRight={t('Enter code manually')}
            onClick={handleOnProceed}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default CardlessTransactionScreen
