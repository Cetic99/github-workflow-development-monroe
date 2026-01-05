import ScreenContainer from '@ui/components/screen-container'
import Receipt from '@ui/assets/images/atm/reciept.svg'
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding-top: 6.25rem;
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
    }

    h2 {
      font-size: 4.375rem;
      color: var(--secondary-dark);
      font-weight: 800;
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
    margin-top: 3.125rem;
    h2 {
      font-size: 3.438rem;
      color: black;
      font-weight: 800;
      line-height: 3.438rem;
    }
  }
`

const TakeReceiptScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/check-balance')
  }
  const onAccept = () => {
    navigate('/atm/')
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
          <img src={Receipt} alt="logo" />

          <h1>
            Please take your
            <br /> receipt
          </h1>
        </div>
        <div className="content">
          <h2>Do you want another transaction?</h2>
        </div>

        <div className="footer">
          <CircleButton
            icon={(props) => <IconClose {...props} />}
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
            onClick={onAccept}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default TakeReceiptScreen
