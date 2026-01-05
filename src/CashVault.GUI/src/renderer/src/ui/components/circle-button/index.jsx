/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const ColorMap = {
  dark: {
    bg: 'var(--primary-dark)',
    text: 'black',
    innerText: 'white',
    icon: 'white'
  },
  medium: {
    bg: 'var(--text-medium)',
    text: '#515151',
    innerText: 'white',
    icon: 'white'
  },
  light: {
    bg: 'var(--primary-light)',
    text: 'white',
    innerText: 'var(--primary-dark)',
    icon: 'var(--primary-dark)'
  },
  inverted: {
    bg: 'white',
    text: 'white',
    innerText: 'var(--primary-dark)',
    icon: 'var(--primary-dark)'
  },
  transparent: {
    bg: 'transparent',
    text: 'var(--primary-dark)',
    innerText: 'var(--primary-dark)',
    icon: 'var(--primary-dark)'
  },
  white: {
    bg: 'white',
    text: 'white',
    innerText: 'var(--primary-dark)',
    icon: 'var(--primary-dark)'
  },
  secondary: {
    bg: 'var(--secondary-dark)',
    text: 'white',
    innerText: 'white',
    icon: 'white'
  }
}

const Container = styled.button`
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.25s ease;
  background-color: ${(p) => (p.outlined ? 'transparent' : ColorMap[p.color].bg)};
  border-radius: 50%;
  border: ${(p) => (p.outlined ? `5px solid ${ColorMap[p.color].text}` : 'none')};
  box-shadow: ${(p) =>
    !p.outlined && p.shadow === true ? `${ColorMap[p.color].bg} 0px 5px 15px` : 'none'};

  &:disabled {
    background-color: ${(p) => (p.outlined ? 'transparent' : '#1d4a3e4d')};
    border-color: ${(p) => (p.outlined ? '#1d4a3e4d' : 'none')};
    box-shadow: ${(p) => (!p.outlined && p.shadow === true ? `#1d4a3e4d 0px 5px 15px` : 'none')};
  }

  &:active {
    filter: brightness(80%);
  }

  & .inner-text {
    font-size: 1.5rem;
    font-weight: 600;
    color: ${(p) => ColorMap[p.color].innerText};
  }

  ${(p) => {
    if (p.size === 's') {
      return `
        min-width: 4.063rem;
        min-height: 4.063rem;
        max-width: 4.063rem;
        max-height: 4.063rem;`
    }
    if (p.size === 'm') {
      return `
        min-width: 5.625rem;
        min-height: 5.625rem;
        max-width: 5.625rem;
        max-height: 5.625rem;`
    }
    if (p.size === 'l') {
      return `
        min-width: 7.5rem;
        min-height: 7.5rem;
        max-width: 7.5rem;
        max-height: 7.5rem;`
    }
  }}
`

const OuterContainer = styled.div`
  display: inline-flex;
  gap: 1.25rem;
  align-items: center;
  color: ${(p) => ColorMap[p.color].text};
  padding-left: 1rem;
  padding-bottom: 1rem;

  & .text-left {
    text-align: right;
  }

  & .text-right {
    text-align: left;
  }

  ${(p) => {
    if (p.size === 's' || p.size === 'm') {
      return `
        font-weight: 600 !important;
        font-size: 0.875rem;
        line-height: 1.125rem;
        letter-spacing: 4%;
        text-transform: uppercase;`
    }
    if (p.size === 'l') {
      return `
        font-weight: 500 !important;
        font-size: 1.75rem;
        line-height: 1.875rem;
        letter-spacing: -2%;`
    }
  }}

  ${(p) => {
    if (p.disabled) {
      return `color: var(--bg-medium);`
    }
  }}
`

const CircleButton = ({
  disabled = false,
  icon = () => <></>,
  color = 'dark',
  shadow = false,
  outlined = false,
  textLeft,
  textRight,
  textInner,
  onClick = () => {},
  size = 's',
  className = ''
}) => {
  return (
    <OuterContainer className={className} color={color} size={size} disabled={disabled}>
      {textLeft && <span className="text text-left">{textLeft}</span>}

      <Container
        disabled={disabled}
        onClick={onClick}
        color={color}
        size={size}
        shadow={shadow}
        outlined={outlined}
      >
        {icon && icon({ color: outlined ? ColorMap[color].text : ColorMap[color].icon, size: 'l' })}
        {textInner && <span className="inner-text">{textInner}</span>}
      </Container>

      {textRight && <span className="text text-right">{textRight}</span>}
    </OuterContainer>
  )
}

export default CircleButton
