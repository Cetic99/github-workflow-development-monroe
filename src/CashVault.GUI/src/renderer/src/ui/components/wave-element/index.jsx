/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  width: 100%;
  height: 6.813rem;
  background-color: transparent;
  position: relative;
  z-index: 1;
  & .wave-svg {
    z-index: 5;
    position: absolute;
    bottom: 0rem;
    pointer-events: none;

    path {
      fill: var(--bg-medium);

      ${(p) =>
        p.bgImage &&
        `
          fill: var(--primary-dark);
        `}
    }
    ${(p) => {
      if (p.position === 'bottom-right') {
        return `
                right: 0;
                transform: scale(-1, 1);
            `
      }
    }}

    ${(p) => {
      if (p.position === 'bottom-left') {
        return `
                left: 0;
            `
      }
    }}

    ${(p) => {
      if (p.position === 'top-right') {
        return `
                right: 0;
                transform: rotate(180deg) scale(1);
            `
      }
    }}

    ${(p) => {
      if (p.position === 'top-left') {
        return `
                left: 0;
                transform: scale(1, -1);
            `
      }
    }}
  }

  & .filler {
    background: ${(p) => p.color};
    position: absolute;
    top: 0;
    width: 50%;
    height: 100%;
    z-index: 2;

    ${(p) => {
      if (p.position === 'bottom-right') {
        return `
                left: 0;
            `
      }

      if (p.position === 'bottom-left') {
        return `
                right: 0;
            `
      }

      if (p.position === 'top-right') {
        return `
                left: 0;
            `
      }

      if (p.position === 'top-left') {
        return `
                right: 0;
            `
      }
    }}
  }

  & .element-content {
    z-index: 3;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: ${(p) => p.backgroundColor};
    ${(p) => {
      if (p.bgImage) {
        return `
               background: transparent;
               z-index: 5;
           `
      }
    }}
  }
`

const HeaderWave = () => {
  return (
    <svg
      className="wave-svg"
      viewBox="0 0 1019.0048780487805 52.458000000000006"
      fill="none"
      width="63.938rem"
      height="3.5rem"
    >
      <path d="M336.091 55.0236C315.064 55.0236 295.41 44.5152 281.522 28.5986L276.215 22.5058C263.734 8.19792 245.759 0 226.851 0H0V1249H1024V55.0236H336.091Z" />
    </svg>
  )
}

const InvertedHeaderWave = ({ color }) => {
  return (
    <svg
      className="wave-svg"
      width="63.938rem"
      height="6.813rem"
      viewBox="0 0 1023 109"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M224.909 53.9764C245.936 53.9764 265.59 64.4848 279.478 80.4014L284.785 86.4942C297.266 100.802 315.241 109 334.148 109H1023V0H0V53.9764H224.909Z"
        fill={color}
      />
    </svg>
  )
}

const WaveElement = ({
  children,
  color = '#1D4A3E',
  position = 'top-left',
  bgImage = true,
  backgroundColor = 'var(--primary-dark)'
}) => {
  return (
    <Container
      color={color}
      className="wave-element"
      position={position}
      bgImage={bgImage}
      backgroundColor={backgroundColor}
    >
      {bgImage ? <InvertedHeaderWave /> : <HeaderWave />}
      <div className="filler" />
      {children && <div className="element-content">{children}</div>}
      <div className="wave-expansion"></div>
    </Container>
  )
}

export default WaveElement
