import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import IconQrCode from '@ui/components/icons/IconQrCode'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
    margin-bottom: 4rem;
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
      font-size: 4.375rem;
      line-height: 4.375rem;
      font-weight: 800;
      color: var(--secondary-dark);
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
    h2 {
      font-size: 3.438rem;
      line-height: 3.438rem;
      font-weight: 800;
      color: #020605;
    }
  }
`

const CardlessAmountScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/')
  }
  const handleOnProceed = () => {
    navigate('/atm/take-money')
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
            Amount for
            <br /> withdrawal:
          </h1>
          <h2>650 EUR</h2>
        </div>

        <div className="content">
          <h2>Do you want to continue?</h2>
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
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            onClick={handleOnProceed}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default CardlessAmountScreen
