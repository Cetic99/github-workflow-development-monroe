/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import { keyframes } from '@emotion/react'
import { useNavigate } from 'react-router-dom'
import LogoMedium from '../../components/icons/LogoMedium'

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

const Container = styled.div`
  width: 100dvw;
  height: 100dvh;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 1;

  animation: ${fadeIn} 0.5s ease-in;
`

const Content = styled.div`
  background-color: #004b3e;
  height: 7.75rem;
  flex-grow: 100;
  width: 100%;
  padding: 0 3.5rem 4rem 3.5rem;
`

const Footer = styled.div`
  height: 10rem;
  position: relative;

  & .wave {
    position: absolute;
    top: -3.5rem;
    left: 0;
    right: 0;
    width: 100%;
  }
`

const Main = styled.div`
  height: 100%;
  width: 100%;
  overflow: hidden;
  display: flex;
  flex-direction: column;

  & .scroll-container {
    flex: 1;
    overflow: auto;
    flex-direction: column;

    mask-image: linear-gradient(to bottom, transparent, black 15%, black 85%, transparent);
  }
`

const AppLogo = styled.div`
  position: absolute;
  padding: 0 0 0 3.5rem;
  top: 0;
  left: 0;
  z-index: 3;
`

const ScreenContainerBottom = ({ children }) => {
  const { t } = useTranslation()

  return (
    <Container>
      <Content>
        <Main className="screen-container-content">
          <div className="scroll-container">{children}</div>
        </Main>
      </Content>

      <Footer>
        <div className="wave">
          <Wave />
        </div>

        <AppLogo>
          <LogoMedium />
        </AppLogo>
      </Footer>
    </Container>
  )
}

export default ScreenContainerBottom
