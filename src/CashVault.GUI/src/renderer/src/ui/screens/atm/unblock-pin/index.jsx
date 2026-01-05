import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import styled from '@emotion/styled'
import IconLockOpen from '@ui/components/icons/IconLockOpen'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'

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

const UnblockPinScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate('/atm/')
  }
  const handleOnProceed = () => {
    navigate('/atm/loading?to=transaction-complete?success=true')
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
          <IconLockOpen size="xl" color="var(--secondary-dark)" />

          <h3>Unblock PIN</h3>
          <h1>
            Do you want to
            <br />
            unblock your PIN?
          </h1>
          <h2>To unblock PIN, please click ‘Proceed’.</h2>
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

export default UnblockPinScreen
