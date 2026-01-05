/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import RetailHeader from '@ui/components/retail-header'

const ColorMap = {
  dark: {
    bg: '#0F0E18'
  },
  light: {
    bg: 'var(--bg-medium)'
  },
  secondary: {
    bg: '#004B3E'
  }
}

const Container = styled.div`
  height: 100dvh;
  width: 100dvw;
  background-color: ${(p) => ColorMap[p.color].bg};

  overflow-y: auto;

  & .children > * {
    padding: 0 3rem;
  }
`

// eslint-disable-next-line react/prop-types
const RetailContainer = ({
  children,
  color = 'light',
  hasBackButton = true,
  hasHomeButton = true,
  hasFaqButton = true,
  hasCartButton = true,
  hasExitButton = false
}) => {
  return (
    <Container color={color}>
      <RetailHeader
        color={color}
        hasBackButton={hasBackButton}
        hasHomeButton={hasHomeButton}
        hasFaqButton={hasFaqButton}
        hasCartButton={hasCartButton}
        hasExitButton={hasExitButton}
      />
      <div className="children">{children}</div>
    </Container>
  )
}

export default RetailContainer
