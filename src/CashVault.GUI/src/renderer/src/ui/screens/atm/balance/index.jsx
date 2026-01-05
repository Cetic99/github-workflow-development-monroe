import styled from '@emotion/styled'
import IconReceipt from '@ui/components/icons/IconReceipt'
import CircleButton from '@ui/components/circle-button'
import ScreenContainer from '@ui/components/screen-container'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: space-between;
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
    h2 {
      font-size: 3.438rem;
      color: black;
      font-weight: 800;
      margin-top: 1.5rem;
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

const BalanceScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/')
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
          <IconReceipt size="xl" color="var(--secondary-dark)" />

          <h3>Check balance</h3>
          <h1>
            Your available
            <br />
            balance:
          </h1>
          <h2>5.439,50 EUR</h2>
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

export default BalanceScreen
