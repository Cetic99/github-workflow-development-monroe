import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import IconSettings from '@ui/components/icons/IconSettings'
import { useLocation, useNavigate } from 'react-router-dom'
import { useEffect } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 50vh;
  overflow: auto;
  justify-content: center;
  h1 {
    font-size: 4.375rem;
    font-weight: 800;
    line-height: 4.375rem;
    margin: 1.5rem 0;
  }

  h2 {
    font-size: 1.625rem;
    color: var(--primary-light);
    font-weight: 400;
  }

  .cogwheel {
    width: fit-content;
    height: fit-content;
    animation: spin 8s linear infinite;
  }

  @keyframes spin {
    from {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(360deg);
    }
  }
`

const LoadingScreen = () => {
  const navigate = useNavigate()
  const location = useLocation()

  const queryParams = new URLSearchParams(location.search)
  const redirectTo = queryParams.get('to') || '/atm/take-card/money'

  useEffect(() => {
    const timeout = setTimeout(() => {
      navigate('/atm/' + redirectTo)
    }, 5000)

    return () => clearTimeout(timeout)
  }, [navigate, redirectTo])

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
    >
      <Container>
        <div className="cogwheel">
          <IconSettings size="xl" color="var(--primary-light)" />
        </div>
        <h1>
          One moment,
          <br /> please
        </h1>
        <h2>We are preparing your transaction...</h2>
      </Container>
    </ScreenContainer>
  )
}

export default LoadingScreen
