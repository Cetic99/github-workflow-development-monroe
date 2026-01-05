/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Logo from '../../assets/images/Logo.svg'
import WaveElement from '../wave-element'
import ScreenHeader from '@ui/components/screen-header'
import Idle from '../../assets/images/roulette1.gif'
import { useNavigate } from 'react-router-dom'
import Loading from '@ui/components/loading'

const Container = styled.main`
  display: flex;
  flex-direction: column;
  width: 100dvw;
  height: 100dvh;
`

const Header = styled.div`
  width: 100dvw;
  height: 14.375rem;
  height: ${(p) => (p.hasLogo ? '14.375rem' : 'unset')};
  background-color: var(--bg-medium);
  position: relative;

  .logo-container {
    position: absolute;
    top: 6.25rem;
    left: 3rem;
    background: var(--bg-medium);
    z-index: 5;
    ${(p) => p.hasFadeLogo && `border-radius: 0.3rem;`}

    .logo {
      width: 11.29rem;
      height: auto;
      display: block;
      ${(p) =>
        p.hasFadeLogo &&
        `
        border-radius: 0.3rem;
        box-shadow:
        0 0 0.5rem var(--bg-medium),
        0 0 1rem var(--bg-medium),
        0 0 1.5rem var(--bg-medium);
        `}
    }
  }
  .bg-image-container {
    margin-top: -3.5rem;
    position: relative;
    max-height: revert;
    overflow: visible;

    .bg-image {
      width: 100%;
      max-height: 25rem;
      object-fit: cover;

      @media (max-height: 832px) {
        display: none;
      }

      @media (min-height: 833px) {
        max-height: 25rem;
      }

      @media (min-height: 1500px) {
        max-height: unset;
      }
    }

    .wave {
      position: absolute;
      bottom: -3.125rem;
      height: auto;
      width: 100dvw;
    }

    .bg-img-logo-container {
      position: absolute;
      bottom: -7rem;
      left: 3.25rem;

      .bg-logo {
        width: 11.25rem;
        height: auto;
      }

      @media (max-height: 832px) {
        bottom: -11rem;
      }
    }
  }
`

const Main = styled.div`
  height: calc(100dvh - 14.375rem);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  flex-grow: 100;
  background-color: var(--bg-medium);
  padding: ${(p) => (p.padding ? '3.25rem 5.25rem' : '3.25rem 0 0 0')};

  & .main-content {
    flex-grow: 100;
    overflow: ${(p) => (p.overflow ? 'auto' : 'hidden')};
    padding: 0 3rem;
    padding-bottom: 1.5rem;
    overflow: auto;
  }
`

const BottomWaveContainer = styled.div`
  position: absolute;
  bottom: 8rem;
  width: 100%;

  .logo-container {
    position: absolute;
    bottom: -7rem;
    left: 3.25rem;

    .logo {
      width: 11.25rem;
      height: auto;
    }
  }
`

const BottomWaveMain = styled.div`
  height: calc(100dvh - 14.375rem);
  background-color: ${(p) => p.containerColor} !important;
  color: var(--text-light);

  .screen-header {
    background-color: ${(p) => p.containerColor} !important;
    height: 6.375rem;
  }

  .main-content {
    flex-grow: 100;
    overflow: auto;
    padding: 1.25rem 5.25rem 3.25rem 5.25rem;
  }
`

const Wave = () => {
  return (
    <svg
      width="1024"
      height="106"
      viewBox="0 0 1024 106"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M336.091 55.7823C315.064 55.7823 295.41 45.129 281.522 28.9929L276.215 22.8161C263.734 8.31095 245.759 0 226.852 0H0V106H1024V55.7823H336.091Z"
        fill="var(--bg-medium)"
      />
    </svg>
  )
}

const ScreenContainer = ({
  children,
  isBottomWave = false,
  hasOnGoBack = true,
  hasLogo = true,
  hasLanguageSwitcher = true,
  hasBackgroundImage = false,
  hasExitButton = false,
  hasMasterAuthButton = true,
  hasUserButton = true,
  hasSettingsButton = false,
  hasTimeDate = true,
  urlPrefix,
  backgroundColor = 'var(--primary-dark)',
  hasAdminLoginButton = true,
  hasFadeLogo = false,
  isLoading = false,
  loadingText = 'Loading...',
  hasNavigate = true,
  hasExperienceSelector = true,
  onGoBack = () => {}
}) => {
  const navigate = hasNavigate ? useNavigate() : null

  if (isBottomWave === true || isLoading === true) {
    return (
      <Container hasLogo={hasLogo}>
        <BottomWaveMain className="screen-content" containerColor={backgroundColor}>
          <Header className="screen-header" containerColor={backgroundColor}>
            <ScreenHeader
              hasOnGoBack={hasOnGoBack}
              hasLanguageSwitcher={hasLanguageSwitcher}
              hasExitButton={hasExitButton}
              hasMasterAuthButton={hasMasterAuthButton}
              hasUserButton={hasUserButton}
              hasSettingsButton={hasSettingsButton}
              hasTimeDate={hasTimeDate}
              urlPrefix={urlPrefix}
              onGoBackProvided={onGoBack}
              hasAdminLoginButton={hasAdminLoginButton}
              hasExperienceSelector={hasExperienceSelector}
            />
          </Header>

          {isLoading ? (
            <Loading text={loadingText} />
          ) : (
            <div className="main-content">{children}</div>
          )}
        </BottomWaveMain>
        <BottomWaveContainer className="screen-content">
          <WaveElement position="bottom-left" bgImage={false} backgroundColor={backgroundColor}>
            <div
              className="logo-container"
              onClick={() => navigate(urlPrefix ? `/${urlPrefix}/` : '/')}
            >
              <img className="logo" src={Logo} />
            </div>
          </WaveElement>
        </BottomWaveContainer>
      </Container>
    )
  }
  return (
    <Container hasLogo={hasLogo}>
      <Header className="screen-header" hasFadeLogo={hasFadeLogo}>
        <WaveElement
          position="bottom-left"
          bgImage={hasBackgroundImage}
          backgroundColor={backgroundColor}
        >
          <ScreenHeader
            hasOnGoBack={hasOnGoBack}
            hasLanguageSwitcher={hasLanguageSwitcher}
            hasExitButton={hasExitButton}
            hasUserButton={hasUserButton}
            hasMasterAuthButton={hasMasterAuthButton}
            hasSettingsButton={hasSettingsButton}
            urlPrefix={urlPrefix}
            onGoBackProvided={onGoBack}
            hasAdminLoginButton={hasAdminLoginButton}
            hasExperienceSelector={hasExperienceSelector}
          />
        </WaveElement>
        {hasBackgroundImage === true && (
          <div className="bg-image-container">
            <div className="wave">
              <Wave />
            </div>
            <div
              className="bg-img-logo-container"
              onClick={() => navigate(urlPrefix ? `/${urlPrefix}/` : '/')}
            >
              <img className="bg-logo" src={Logo} />
            </div>
            <img className="bg-image" src={Idle} />
          </div>
        )}

        {hasBackgroundImage === false && hasLogo && (
          <>
            <div
              className="logo-container"
              onClick={() => navigate(urlPrefix ? `/${urlPrefix}/` : '/')}
            >
              <img className="logo" src={Logo} />
            </div>
          </>
        )}
      </Header>

      <Main className="screen-content">
        <div className="main-content">{children}</div>
      </Main>
    </Container>
  )
}

export default ScreenContainer
