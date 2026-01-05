/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Logo from '@ui/assets/images/LogoWhite.svg'
import BackgroundImage from '@ui/assets/images/atm/idle-background.png'
import CreditCardImage from '@ui/assets/images/atm/credit-card.svg'
import { useTranslation } from '@domain/administration/stores'
import { keyframes } from '@emotion/react'
import { useNavigate } from 'react-router-dom'
import IconWifiOn from '@ui/components/icons/IconWifiOn'
import IconUser from '@ui/components/icons/IconUser'

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

const CreditCardButton = styled.div`
  display: flex;
  align-items: center;
  gap: 1.5rem;

  h5 {
    font-size: 1.75rem;
    font-weight: 600;
  }

  & .credit-card-container {
    padding: 1rem;
    background-color: #2bd6b5;
    display: flex;
    justify-content: center;
    border-radius: 100%;
    position: relative;
    overflow: hidden;

    width: 7.5rem;
    height: 7.5rem;

    & .credit-card {
      width: 5rem;
      position: absolute;
      bottom: 0;
    }
  }
`
const CreditCard = () => {
  return (
    <CreditCardButton>
      <div className="credit-card-container">
        <img src={CreditCardImage} alt="Credit card" className="credit-card" />
      </div>
      <h5>
        Insert your
        <br />
        card
      </h5>
    </CreditCardButton>
  )
}

const CardlessContainer = styled.div`
  display: flex;
  align-items: center;
  gap: 1.5rem;

  h5 {
    font-size: 1.75rem;
    font-weight: 600;
  }
`
const Cardless = () => {
  return (
    <CardlessContainer>
      <div className="icon">
        <svg
          width="37"
          height="37"
          viewBox="0 0 37 37"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M13.2043 11.0036V2.15168C13.2043 0.982421 12.2522 0.0345459 11.0776 0.0345459L2.18535 0.0345459C1.01077 0.0345459 0.0585775 0.982421 0.0585775 2.15168V11.0036C0.0585775 12.1729 1.01077 13.1207 2.18535 13.1207H11.0776C12.2522 13.1207 13.2043 12.1729 13.2043 11.0036Z"
            fill="#E7ECE4"
          />
          <path
            d="M8.10324 3.77539H5.15995C4.4182 3.77539 3.81689 4.37397 3.81689 5.11236V8.04231C3.81689 8.7807 4.4182 9.37928 5.15995 9.37928H8.10324C8.84499 9.37928 9.4463 8.7807 9.4463 8.04231V5.11236C9.4463 4.37397 8.84499 3.77539 8.10324 3.77539Z"
            fill="#004034"
          />
          <path
            d="M13.1458 34.8829V26.031C13.1458 24.8617 12.1936 23.9138 11.019 23.9138H2.12676C0.952175 23.9138 -1.62125e-05 24.8617 -1.62125e-05 26.031V34.8829C-1.62125e-05 36.0521 0.952175 37 2.12676 37H11.019C12.1936 37 13.1458 36.0521 13.1458 34.8829Z"
            fill="#E7ECE4"
          />
          <path
            d="M8.04416 27.6553H5.10087C4.35912 27.6553 3.75781 28.2539 3.75781 28.9922V31.9222C3.75781 32.6606 4.35912 33.2592 5.10087 33.2592H8.04416C8.78591 33.2592 9.38721 32.6606 9.38721 31.9222V28.9922C9.38721 28.2539 8.78591 27.6553 8.04416 27.6553Z"
            fill="#004034"
          />
          <path
            d="M37 11.0036V2.15168C37 0.98242 36.0478 0.0345459 34.8732 0.0345459L25.981 0.0345459C24.8064 0.0345459 23.8542 0.98242 23.8542 2.15168V11.0036C23.8542 12.1729 24.8064 13.1207 25.981 13.1207H34.8732C36.0478 13.1207 37 12.1729 37 11.0036Z"
            fill="#E7ECE4"
          />
          <path
            d="M31.8991 3.77539H28.9559C28.2141 3.77539 27.6128 4.37397 27.6128 5.11236V8.04231C27.6128 8.7807 28.2141 9.37928 28.9559 9.37928H31.8991C32.6409 9.37928 33.2422 8.7807 33.2422 8.04231V5.11236C33.2422 4.37397 32.6409 3.77539 31.8991 3.77539Z"
            fill="#004034"
          />
          <path d="M20.896 0H16.7383V4.521H20.896V0Z" fill="#E7ECE4" />
          <path opacity="0.43" d="M20.896 4.521H16.7383V8.21447H20.896V4.521Z" fill="#004034" />
          <path d="M20.896 4.521H16.7383V8.21447H20.896V4.521Z" fill="#004034" />
          <path d="M6.31142 16.6837H1.0022V20.8226H6.31142V16.6837Z" fill="#E7ECE4" />
          <path
            opacity="0.43"
            d="M10.0218 16.6837H6.31152V20.8226H10.0218V16.6837Z"
            fill="#004034"
          />
          <path d="M10.0218 16.6837H6.31152V20.8226H10.0218V16.6837Z" fill="#004034" />
          <path d="M20.896 8.21448H16.7383V16.6837H20.896V8.21448Z" fill="#E7ECE4" />
          <path d="M20.896 20.8226H16.7383V32.6666H20.896V20.8226Z" fill="#E7ECE4" />
          <path d="M29.9825 29.1074H25.8247V32.5033H29.9825V29.1074Z" fill="#E7ECE4" />
          <path d="M29.9825 20.8226H25.8247V24.9686H29.9825V20.8226Z" fill="#E7ECE4" />
          <path d="M25.8247 24.9685H20.7617V29.1074H25.8247V24.9685Z" fill="#E7ECE4" />
          <path d="M36.5807 24.9685H29.9824V29.1074H36.5807V24.9685Z" fill="#E7ECE4" />
          <path d="M29.9606 24.9685H25.8247V29.0857H29.9606V24.9685Z" fill="#004034" />
          <path
            d="M29.9826 32.5039V36.5699H25.8248V32.5039H20.8872V36.6428H36.7056V32.5039H29.9826Z"
            fill="#E7ECE4"
          />
          <path d="M29.9825 32.5039H25.8247V36.655H29.9825V32.5039Z" fill="#004034" />
          <path d="M16.7383 16.6837H10.0217V20.8226H16.7383V16.6837Z" fill="#E7ECE4" />
          <path d="M26.0139 16.6837H20.8962V20.8226H26.0139V16.6837Z" fill="#E7ECE4" />
          <path d="M20.896 16.6837H16.7383V20.8226H20.896V16.6837Z" fill="#004034" />
          <path d="M33.8182 16.6837H29.7241V20.8226H33.8182V16.6837Z" fill="#E7ECE4" />
          <path d="M29.724 16.6837H26.0137V20.8226H29.724V16.6837Z" fill="#004034" />
          <path d="M29.724 16.6837H26.0137V20.8226H29.724V16.6837Z" fill="#004034" />
        </svg>
      </div>
      <h5>
        Cardless
        <br />
        transaction
      </h5>
    </CardlessContainer>
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

const Indicators = styled.div`
  position: absolute;
  top: 1.938rem;
  right: 1.5rem;
  z-index: 3;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
`

const Background = styled.div`
  position: absolute;
  width: 100%;
  height: 80%;
  top: 0;
  left: 0;

  & img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  & .wave {
    display: block;
    position: absolute;
    width: 100%;
    bottom: 0;
    left: 0;
    overflow: hidden;
    & .wave-svg {
      min-width: 1600px; /* širi od ekrana */
      max-width: none;
    }
  }

  & .background-text {
    display: flex;
    flex-direction: column;
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
      font-style: bold;
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

    & .tag {
      text-transform: uppercase;
      color: #2bd6b5;
      font-weight: bold;
      font-size: 0.688rem;
      width: 70%;
      margin: 0 auto;
      display: flex;
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
  display: flex;
  justify-content: space-evenly;
  align-items: center;

  & .buttons-seperator {
    text-transform: uppercase;
    color: var(--secondary-dark);
    font-size: 1rem;
    font-weight: bold;
  }

  & .icon {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 7.5rem;
    width: 7.5rem;
    border-radius: 100%;
    background-color: var(--primary-dark);
  }
`

const IdleScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const navigateToCreditCard = () => {
    navigate('/atm/select-language')
  }

  const navigateToCardless = () => {
    navigate('/atm/cardless-transaction')
  }

  return (
    <Container>
      <AppLogo>
        <img src={Logo} />
      </AppLogo>

      <Indicators>
        <IconUser color="#2BD6B5" />
        <IconWifiOn color="var(--bg-medium)" />
      </Indicators>

      <Background>
        <img src={BackgroundImage} />

        <div className="background-text">
          <span className="tag">Promo box</span>
          <span className="main">Za sve uspehe mi smo tu za Vas!</span>
          <span className="secondary">Saznaj više</span>
        </div>

        <div className="wave">
          <Wave />
        </div>
      </Background>

      <Content>
        <div onClick={navigateToCreditCard}>
          <CreditCard />
        </div>
        <span className="buttons-seperator">OR</span>
        <div onClick={navigateToCardless}>
          <Cardless />
        </div>
      </Content>
    </Container>
  )
}

export default IdleScreen
