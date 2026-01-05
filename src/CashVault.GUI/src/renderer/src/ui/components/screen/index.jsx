/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useScreenHeader } from '@domain/global/stores'
import { useOperatorName } from '@domain/global/stores'
import { useAppMode } from '@domain/global/stores'
import { AppMode } from '@domain/global/constants'
import ScreenHeader from '@ui/components/screen-header'
import WaveElement from '@ui/components/wave-element'
import Logo from '@ui/assets/images/LogoSmall.svg'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  position: relative;
  max-height: 100dvh;
  max-width: 100dvw;
  flex-grow: 100;
`

const Header = styled.div`
  height: 14.375rem;
  background-color: var(--bg-medium);
  position: relative;

  .logo-container {
    position: absolute;
    top: 8.25rem;
    left: 3.25rem;
  }
`

const Main = styled.div`
  height: calc(100dvh - 14.375rem);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  flex-grow: 100;
  background-color: var(--bg-medium);
  padding: 1.25rem 5.25rem 3.25rem 5.25rem;

  ${(p) => {
  if (p.center) {
    return `
        display: flex;
        align-items: center;
        justify-content: center;
      `
  }
}}

  & .main-content {
    flex-grow: 100;
    overflow: auto;
    scrollbar-width: none;
  }
`

const NavigationContainer = styled.div`
  position: absolute;
  top: 7.5rem;
  left: 10.25rem;
  width: calc(100dvw - 20rem);
  display: flex;
  justify-content: space-between;

  .header-title {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .title {
    color: var(--secondary-dark);
    font-weight: 600;
    font-size: 2.5rem;
  }

  .user {
    color: black;
    font-weight: 700;
    font-size: 2.5rem;
    line-height: 2.5rem;
    text-align: right;
    margin-left: auto;

    .welcome {
      color: var(--secondary-dark);
      font-size: 1rem;
      line-height: 1rem;
      letter-spacing: 4%;
      text-transform: uppercase;
    }
  }
`

const Screen = ({ children, isMenu = false }) => {
  const header = useScreenHeader()
  const operatorName = useOperatorName()
  const appMode = useAppMode()
  const navigate = useNavigate()
  const {t} = useTranslation()

  return (
    <Container>
      <Header className="screen-header">
        <WaveElement position="bottom-left">
          <ScreenHeader hasExitButton={true} hasOnGoBack={false} />
        </WaveElement>

        <div className="logo-container" onClick={() => navigate('/')}>
          <img className="logo" src={Logo} />
        </div>

        <NavigationContainer>
          <div className="header-title">
            <IconRightHalfArrow size="l" color="var(--primary-dark)" />
            <div className="title">{header}</div>
          </div>
          {operatorName && (
            <div className="user">
              <div className="welcome">{t("welcome")}</div>
              <div>{operatorName}</div>
            </div>
          )}
        </NavigationContainer>
      </Header>

      <Main className="screen-content" center={isMenu}>
        <div className="main-content">{children}</div>
      </Main>
    </Container>
  )
}

export default Screen
