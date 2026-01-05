/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Logo from '@ui/assets/images/LogoWhite.svg'
import BackgroundImage from '@ui/assets/images/parcel-locker/idle-background.jpg'
import { useTranslation } from '@domain/administration/stores'
import { keyframes } from '@emotion/react'
import { useNavigate } from 'react-router-dom'

const fadeIn = keyframes`
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
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

const Touch = () => {
  return (
    <svg width="54" height="84" viewBox="0 0 54 84" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path
        d="M12.0881 28.4739C10.775 26.5724 10.007 24.2702 10.007 21.7905C10.007 15.2671 15.3254 9.97173 21.8772 9.97173C28.4289 9.97173 33.7473 15.2671 33.7473 21.7905C33.7473 23.9586 33.1592 25.991 32.134 27.7388M37.3897 34.1588C40.1197 30.7702 41.7538 26.4685 41.7538 21.7905C41.7538 10.8674 32.8473 2 21.8772 2C10.907 2 2 10.8679 2 21.7905C2 25.9396 3.28544 29.7924 5.48052 32.9749C6.90329 35.038 8.70875 36.8192 10.7947 38.2178M13.0505 67.6462C12.0759 64.8867 10.5877 62.3349 8.66351 60.125C6.99164 58.2017 4.99243 55.9054 3.50047 54.1915C2.20492 52.7044 2.07931 50.5342 3.19335 48.9077C3.36262 48.6608 3.53667 48.4069 3.71232 48.1509C4.441 47.0868 5.60667 46.4005 6.89424 46.2775C8.1818 46.1546 9.45766 46.6077 10.3764 47.5134C13.4598 50.5559 17.3422 54.3865 17.3422 54.3865C17.3422 54.3865 17.199 33.269 17.1325 23.3984C17.1234 22.082 17.6424 20.8164 18.5739 19.8826C19.5059 18.9488 20.7732 18.4236 22.0949 18.4236C24.8078 18.4236 27.0109 20.6071 27.0237 23.3083C27.0684 32.7729 27.1626 52.6896 27.1626 52.6896C27.1626 52.6896 27.2307 46.7285 27.2792 42.5466C27.3047 40.3117 29.0969 38.4955 31.3409 38.4303C32.4864 38.3969 33.5967 38.8236 34.4228 39.6132C35.2489 40.4028 35.7215 41.4909 35.7348 42.6308C35.7827 46.8414 35.8482 52.6085 35.8482 52.6085C35.8482 52.6085 35.8173 48.2893 35.7934 44.9685C35.779 42.8857 37.4461 41.1777 39.5374 41.1326C41.6287 41.0876 43.3655 42.7305 43.4278 44.8122C43.5316 48.2506 43.6684 52.803 43.6684 52.803C43.6684 52.803 43.6444 50.8792 43.6189 48.8341C43.6034 47.6114 44.1101 46.4397 45.0129 45.6103C45.9151 44.7804 47.1292 44.3718 48.3518 44.4846C50.1094 44.6473 51.4677 46.0915 51.5151 47.8478C51.6732 53.6663 52 65.7007 52 65.7007C51.7413 75.0392 44.092 82.4678 31.3777 81.977C22.8555 81.6479 16.737 78.1597 13.0505 67.6457V67.6462Z"
        stroke="white"
        strokeWidth="4"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

const Container = styled.div`
  width: 100dvw;
  height: 100dvh;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 1;

  animation: ${fadeIn} 0.5s ease-in;
`

const AppLogo = styled.div`
  padding: 3.5rem 0 2rem 3.5rem;
  position: absolute;
  top: 0;
  left: 0;
  z-index: 3;
`

const Background = styled.div`
  position: absolute;
  width: 100%;
  height: 80%;
  top: 0;
  left: 0;

  & img {
    width: 1500px;
    height: 100%;
    object-fit: cover;
  }

  & .wave {
    display: block;
    position: absolute;
    width: 100%;
    bottom: 0;
    left: 0;
  }

  & .background-text {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    position: absolute;
    width: 100%;
    bottom: 10rem;
    left: 0;

    & .main {
      margin: 0 auto;
      display: flex;
      justify-content: center;
      width: 70%;
      font-weight: 700;
      font-style: Bold;
      font-size: 4.375rem;
      line-height: 5rem;
      color: white;
    }

    & .secondary {
      margin: 0 auto;
      display: flex;
      width: 70%;
      font-weight: 400;
      font-size: 1.625rem;
      line-height: 2.25rem;
      color: white;
    }
  }
`

const Content = styled.div`
  position: absolute;
  width: 100%;
  height: 21%;
  bottom: 0;
  left: 0;
  background-color: var(--bg-medium);

  & .content-container {
    display: flex;
    gap: 2rem;
    align-items: center;
    justify-content: center;
    margin: 1rem auto;

    & .icon {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 7.5rem;
      width: 7.5rem;
      border-radius: 100%;
      background-color: var(--primary-dark);
    }

    & .text {
      font-weight: 700;
      font-style: Bold;
      font-size: 3rem;
      line-height: 3.5rem;
    }
  }
`

const IdleScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const navigateToNext = () => {
    navigate('/parcel-locker/select-language')
  }

  return (
    <Container onClick={navigateToNext}>
      <AppLogo>
        <img src={Logo} />
      </AppLogo>

      <Background>
        <img src={BackgroundImage} />

        <div className="background-text">
          <span className="main">{t('Send and receive packages when you feel like it')}</span>
          <span className="secondary">{t('Monroe parcel lockers are here for your comfort')}</span>
        </div>

        <div className="wave">
          <Wave />
        </div>
      </Background>

      <Content>
        <div className="content-container">
          <div className="icon">
            <Touch />
          </div>

          <div className="text">{t('Touch screen to begin')}</div>
        </div>
      </Content>
    </Container>
  )
}

export default IdleScreen
