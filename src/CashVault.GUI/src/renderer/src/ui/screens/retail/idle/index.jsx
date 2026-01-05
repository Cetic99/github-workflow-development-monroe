import styled from '@emotion/styled'
import Logo from '@ui/assets/images/LogoWhite.svg'
import BackgroundImage from '@ui/assets/images/retail/idle-background.png'
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

const Touch = () => {
  return (
    <svg
      width="224"
      height="224"
      viewBox="0 0 224 224"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <g filter="url(#filter0_d_2008_6181)">
        <circle cx="112" cy="112" r="80" fill="#2BD6B5" />
      </g>
      <g filter="url(#filter1_d_2008_6181)">
        <circle cx="112" cy="112" r="60" fill="#2BD6B5" />
      </g>
      <g filter="url(#filter2_d_2008_6181)">
        <circle cx="112" cy="112" r="40" fill="#2BD6B5" />
      </g>
      <g filter="url(#filter3_d_2008_6181)">
        <circle cx="112" cy="112" r="20" fill="white" />
      </g>
      <defs>
        <filter
          id="filter0_d_2008_6181"
          x="0"
          y="0"
          width="224"
          height="224"
          filterUnits="userSpaceOnUse"
          colorInterpolationFilters="sRGB"
        >
          <feFlood floodOpacity="0" result="BackgroundImageFix" />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset />
          <feGaussianBlur stdDeviation="16" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix
            type="matrix"
            values="0 0 0 0 0.168627 0 0 0 0 0.831373 0 0 0 0 0.705882 0 0 0 1 0"
          />
          <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_2008_6181" />
          <feBlend
            mode="normal"
            in="SourceGraphic"
            in2="effect1_dropShadow_2008_6181"
            result="shape"
          />
        </filter>
        <filter
          id="filter1_d_2008_6181"
          x="36"
          y="36"
          width="152"
          height="152"
          filterUnits="userSpaceOnUse"
          colorInterpolationFilters="sRGB"
        >
          <feFlood floodOpacity="0" result="BackgroundImageFix" />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset />
          <feGaussianBlur stdDeviation="8" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0" />
          <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_2008_6181" />
          <feBlend
            mode="normal"
            in="SourceGraphic"
            in2="effect1_dropShadow_2008_6181"
            result="shape"
          />
        </filter>
        <filter
          id="filter2_d_2008_6181"
          x="56"
          y="56"
          width="112"
          height="112"
          filterUnits="userSpaceOnUse"
          colorInterpolationFilters="sRGB"
        >
          <feFlood floodOpacity="0" result="BackgroundImageFix" />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset />
          <feGaussianBlur stdDeviation="8" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0" />
          <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_2008_6181" />
          <feBlend
            mode="normal"
            in="SourceGraphic"
            in2="effect1_dropShadow_2008_6181"
            result="shape"
          />
        </filter>
        <filter
          id="filter3_d_2008_6181"
          x="76"
          y="76"
          width="72"
          height="72"
          filterUnits="userSpaceOnUse"
          colorInterpolationFilters="sRGB"
        >
          <feFlood floodOpacity="0" result="BackgroundImageFix" />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset />
          <feGaussianBlur stdDeviation="8" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0" />
          <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_2008_6181" />
          <feBlend
            mode="normal"
            in="SourceGraphic"
            in2="effect1_dropShadow_2008_6181"
            result="shape"
          />
        </filter>
      </defs>
    </svg>
  )
}

const Container = styled.div`
  width: 100dvw;
  height: 100dvh;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  position: relative;
  z-index: 1;

  background-image: url(${BackgroundImage});
  background-size: cover;

  animation: ${fadeIn} 0.5s ease-in;
`

const AppLogo = styled.div`
  padding: 3.5rem 0 2rem 3.5rem;
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  z-index: 3;
`
const FadeTop = styled.div`
  position: absolute;
  top: 0;
  height: 13rem;
  width: 100%;

  background: linear-gradient(to bottom, rgba(0, 0, 0, 1) 0%, rgba(0, 0, 0, 0) 100%);
`

const Content = styled.div`
  display: flex;
  flex-direction: column;
  z-index: 100;

  h1 {
    text-align: center;
    font-size: 6rem;
    line-height: 6rem;
    color: white;
    font-weight: 800;
  }

  h2 {
    margin-top: 1rem;
    font-size: 2.5rem;
    font-weight: 400;
    line-height: 3rem;
    color: white;
    text-align: center;
  }

  & .touch-container {
    margin-top: 5.688rem;
    display: flex;
    justify-content: center;
  }

  h3 {
    font-weight: 700;
    font-size: 1.625rem;
    line-height: 2.375rem;
    color: white;
    text-transform: uppercase;
    text-align: center;
    margin-bottom: 6rem;
  }
`

const IdleScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const navigateToNext = () => {
    navigate('retail/')
  }

  return (
    <Container>
      <FadeTop />
      <AppLogo>
        <img src={Logo} />
      </AppLogo>

      <Content>
        <h1>{t('Find Your Greatness')}</h1>
        <h2>{t('Rise. Run. Rest. Repeat.')}</h2>

        <div className="touch-container" onClick={() => navigateToNext()}>
          <Touch />
        </div>

        <h3>{t('Touch the screen to start shopping')}</h3>
      </Content>
    </Container>
  )
}

export default IdleScreen
