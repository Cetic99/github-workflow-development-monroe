/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import Logo from '@ui/assets/images/LogoWhite.svg'
import Idle from '@ui/assets/images/roulette1.gif'
import IconTouchTheScreen from '@ui/components/icons/IconTouchTheScreen'
import IconReceipt from '@ui/components/icons/IconReceipt'
import { useTranslation } from '@domain/administration/stores'
import { keyframes } from '@emotion/react'

const fadeIn = keyframes`
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
`

const Container = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  animation: ${fadeIn} 0.5s ease-in;

  .idle-image-container {
    position: relative;
    max-height: 50%;
    overflow: hidden;

    .idle-image {
      width: 100%;
    }

    .wave {
      position: absolute;
      bottom: -3.125rem;
      height: auto;
      width: 100dvw;
    }
  }
`

const MainContainer = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-content: center;
  align-items: center;
  height: 100%;

  .content {
    display: flex;
    gap: 3.5rem;
    width: 100%;
    height: fit-content;
    min-width: 100%;
  }

  margin-top: 4rem;
  padding: 0 5rem 5rem 15%;

  .main-text {
    color: var(--text-dark);
    font-size: 1.5rem;
    font-weight: 700;
    font-size: 4.375rem;
    line-height: 5.25rem;
    max-width: 50rem;
  }

  .right-content {
    display: flex;
    flex-direction: column;
    width: 100%;
    justify-content: space-between;
    padding-bottom: 5rem;
  }

  .options {
    display: flex;
    align-items: center;
    gap: 1.5rem;

    .text-right {
      max-width: 10rem;
      word-break: break-word;
    }

    .or {
      font-weight: 600;
      color: var(--text-default);
    }
  }
`

const Header = styled.div`
  position: relative;

  .logo {
    position: absolute;
    top: 3.25rem;
    left: 3.25rem;
    z-index: 99;
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

const IdleScreen = () => {
  const { t } = useTranslation()

  return (
    <Container isBottomWave={true}>
      <Header>
        <div className="logo-container">
          <img className="logo" src={Logo} />
        </div>
      </Header>

      <div className="idle-image-container">
        <div className="wave">
          <Wave />
        </div>
        <img className="idle-image" src={Idle} />
      </div>

      <MainContainer>
        <div className="content">
          <div className="left-content">
            <IconTouchTheScreen height="136" width="85" color="black" />
          </div>

          <div className="right-content">
            <div className="main-text">{t('Touch screen or insert ticket, bill')}</div>
          </div>
        </div>

        <div className="options">
          <CircleButton
            icon={(props) => <IconReceipt {...props} color="var(--primary-light)" />}
            size="l"
            color="dark"
            textRight={t('Insert your ticket')}
          />
          <div className="or">{t('or')}</div>
          <CircleButton
            icon={(props) => <IconTouchTheScreen {...props} color="var(--primary-light)" />}
            size="l"
            color="dark"
            textRight={t('Touch the screen')}
          />
        </div>
      </MainContainer>
    </Container>
  )
}

export default IdleScreen
