/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import IconLeftArrow from '@ui/components/icons/IconLeftArrow'
import IconHome from '@ui/components/icons/IconHome'
import IconQuestionMark from '@ui/components/icons/IconQuestionMark'
import IconShoppingCart from '@ui/components/icons/IconShoppingCart'
import Logo from '@ui/assets/images/Logo.svg'
import WhiteLogo from '@ui/assets/images/LogoWhite.svg'
import { useNavigate } from 'react-router-dom'
import { globalStore, useIsMultipleRoutersEnabled } from '@domain/global/stores'
import IconLogout from '../icons/IconLogout'
import { useContext } from 'react'
import { HistoryContext } from '@ui/components/history-tracker'

const ColorMap = {
  dark: {
    bg: '#0F0E18',
    logo: WhiteLogo
  },
  light: {
    bg: '#dae1d6',
    logo: Logo
  },
  secondary: {
    bg: '#004237',
    logo: WhiteLogo
  }
}

const Container = styled.div`
  width: 100%;
  position: relative;
  z-index: 10;
  background-color: ${(p) => ColorMap[p.color].bg};
  padding: 2rem 4.5rem 0.5rem 4.5rem;

  margin-bottom: 3.5rem;

  & .bottom-wave {
    position: absolute;
    z-index: 1;
    bottom: -3.5rem;
    left: 50%;
    transform: translateX(-50%);
  }
`

const HeaderButtons = styled.div`
  display: flex;
  justify-content: space-between;

  height: ${(p) => (p.fillSpace === true ? '4.063rem' : '100%')};

  & div {
    display: flex;
    gap: 2rem;
  }
`

const AppLogo = styled.div`
  position: absolute;
  z-index: 2;
  bottom: -1.5rem;
  left: 50%;
  transform: translateX(-50%);

  img {
    width: 7rem;
  }
`

const BottomWave = ({ color }) => {
  return (
    <svg
      className="bottom-wave"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="334.848 143.5532994923858 356.352 61.52284263959391"
      width="356.352"
      height="61.52284263959391"
    >
      <path
        fill={ColorMap[color].bg}
        d="M472 202C450.348 202 431.376 190.531 420.825 173.338C411.795 158.622 398.33 144 381.065 144L8.00001 144C-9.67309 144 -24 129.673 -24 112L-24 31.9998C-24 14.3267 -9.67308 -0.000153675 8.00002 -0.00015213L1018 -6.38327e-05C1035.67 -6.22877e-05 1050 14.3268 1050 31.9999L1050 112C1050 129.673 1035.67 144 1018 144L642.935 144C625.67 144 612.205 158.622 603.175 173.338C592.624 190.531 573.652 202 552 202L472 202Z"
      />
    </svg>
  )
}

const RetailHeader = ({
  color = 'light',
  hasBackButton = true,
  hasHomeButton = true,
  hasFaqButton = true,
  hasCartButton = true,
  hasExitButton = true
}) => {
  const navigate = useNavigate()
  const isMultipleRoutersEnabled = useIsMultipleRoutersEnabled()
  const { setHistoryStack } = useContext(HistoryContext)

  const handleBackNavigate = () => {
    navigate(-1)
  }

  const handleHomeNavigate = () => {
    navigate('/retail/')
  }

  const handleCartNavigate = () => {
    navigate('/retail/cart')
  }

  const handleExperienceSelect = () => {
    setHistoryStack([])
    globalStore.getState().actions.setSelectedRouter(null)
  }

  return (
    <Container color={color}>
      <HeaderButtons
        fillSpace={!hasBackButton && !hasHomeButton && !hasCartButton && !hasCartButton}
      >
        <div>
          {hasBackButton && (
            <CircleButton
              size="s"
              outlined={color === 'dark'}
              color="white"
              icon={(props) => <IconLeftArrow {...props} />}
              onClick={handleBackNavigate}
            />
          )}

          {hasHomeButton && (
            <CircleButton
              size="s"
              outlined={color === 'dark'}
              color="white"
              icon={(props) => <IconHome {...props} />}
              onClick={handleHomeNavigate}
            />
          )}
        </div>
        <div>
          {isMultipleRoutersEnabled && !hasExitButton && (
            <CircleButton
              color="white"
              icon={(props) => <IconLogout {...props} />}
              onClick={handleExperienceSelect}
            />
          )}
          {hasFaqButton && (
            <CircleButton
              size="s"
              outlined={color === 'dark'}
              color="white"
              icon={(props) => <IconQuestionMark {...props} />}
              onClick={() => undefined}
            />
          )}

          {hasCartButton && (
            <CircleButton
              size="s"
              outlined={color === 'dark'}
              color="white"
              icon={(props) => <IconShoppingCart {...props} />}
              onClick={handleCartNavigate}
            />
          )}
        </div>
      </HeaderButtons>

      <AppLogo>
        <img src={ColorMap[color].logo} />
      </AppLogo>

      <BottomWave color={color} />
    </Container>
  )
}

export default RetailHeader
