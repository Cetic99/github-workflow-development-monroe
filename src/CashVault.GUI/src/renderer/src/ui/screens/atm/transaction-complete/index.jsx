import { useLocation, useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import IconThumbUp from '@ui/components/icons/IconThumbUp'
import IconWarning from '@ui/components/icons/IconWarning'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  overflow: auto;
  justify-content: space-between;
  height: 100%;

  h1 {
    font-size: 4.375rem;
    font-weight: 800;
    line-height: 4.375rem;
    margin: 1.5rem 0;
    color: ${(p) => (p.success ? 'white' : 'var(--error-dark)')};
  }

  h2 {
    font-size: 1.625rem;
    color: ${(p) => (p.success ? 'var(--primary-light)' : 'white')};
    font-weight: 400;
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
      font-weight: 800;
      line-height: 3.438rem;
      color: white;
    }
  }
`

const TransactionCompleteScreen = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { t } = useTranslation()

  const queryParams = new URLSearchParams(location.search)
  const success = queryParams.get('success') === 'true'

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
      hasSettingsButton={false}
      hasTimeDate={false}
      isBottomWave={true}
      backgroundColor={success ? 'var(--primary-dark)' : '#222222'}
    >
      <Container success={success}>
        {success ? (
          <IconThumbUp size="xl" color="var(--primary-light)" />
        ) : (
          <IconWarning size="xl" color="var(--error-dark)" />
        )}
        <h1>
          {success ? (
            <>
              Transaction
              <br />
              complete
            </>
          ) : (
            <>
              Transaction
              <br /> not completed
            </>
          )}
        </h1>
        <h2>
          {success
            ? 'You have successfully deposited your money'
            : 'Please try again. Your money has not been deposited. '}
        </h2>

        <div className="content">
          <h2>Do you want another transaction?</h2>
        </div>

        <div className="footer">
          <CircleButton
            icon={(props) => <IconClose {...props} />}
            size="l"
            color="white"
            textRight={t('Back')}
            onClick={onGoBack}
            outlined={true}
          />
          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="white"
            textRight={t('Accept')}
            onClick={onAccept}
            outlined={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default TransactionCompleteScreen
