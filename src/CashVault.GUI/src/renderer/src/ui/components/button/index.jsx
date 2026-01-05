/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useRef } from 'react'
import ConfirmationDialog from '@ui/components/confirmation-dialog'

const ColorMap = {
  dark: {
    bg: 'var(--primary-medium)',
    bgActive: 'var(--primary-dark)',
    text: 'white',
    icon: 'white'
  },
  light: {
    bg: 'var(--text-medium)',
    bgActive: '#798B6E',
    text: 'white',
    icon: 'white'
  },
  gray: {
    bg: '#CCD3C7',
    bgActive: '#a6aba2ff',
    text: 'black',
    icon: 'black'
  },
  secondary: {
    bg: 'var(--secondary-dark)',
    bgActive: '#109e88',
    text: 'white',
    icon: 'white'
  },
  white: {
    bg: 'white',
    bgActive: '#d4d6d3',
    text: 'var(--text-dark)',
    icon: 'var(--text-dark)'
  },
  transparent: {
    bg: 'transparent',
    bgActive: '#109e88',
    text: 'black',
    icon: 'black'
  }
}

const Container = styled.button`
  width: fit-content;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.25s ease;
  background-color: ${(p) => ColorMap[p.color].bg};
  color: ${(p) => ColorMap[p.color].text};
  border-radius: ${(p) => (p.rounded === 'm' ? '10px' : '8px')};
  border: ${(p) => (p.color === 'white' ? '2px solid #CCD3C7' : 'none')};
  font-weight: ${(p) => p.fontWeight};

  padding: 0 1.25rem;
  min-height: 3.5rem;
  gap: 0.75rem;
  font-size: 1.5rem;
  line-height: 1.875rem;

  ${(p) => {
    if (p.size == 'l') {
      return `
          padding: 1.25rem 1.75rem;
          min-height: 3.5rem;
          gap: 1rem;
          font-size: 2rem;
          line-height: 2.5rem;
      `
    }
  }}

  &:active {
    background-color: ${(p) => ColorMap[p.color].bgActive};
  }

  &:disabled {
    opacity: 0.5;
  }
`

const Button = ({
  className = '',
  children,
  disabled = false,
  icon,
  color = 'dark',
  size = 'm',
  rounded = 'm',
  fontWeight = 600,
  onClick = () => {},
  confirmAction = false
}) => {
  const modalRef = useRef()

  const handleOnClick = (e) => {
    if (confirmAction) {
      modalRef?.current?.showModal()
      return
    }

    onClick(e)
  }

  return (
    <>
      {confirmAction && (
        <ConfirmationDialog
          ref={modalRef}
          onConfirm={() => {
            onClick()
            modalRef?.current?.close()
          }}
          onCancel={() => {
            modalRef?.current?.close()
          }}
        />
      )}

      <Container
        size={size}
        className={className}
        disabled={disabled}
        color={color}
        rounded={rounded}
        fontWeight={fontWeight}
        onClick={handleOnClick}
      >
        {icon && icon({ color: ColorMap[color].icon, size: 'm' })}

        {children}
      </Container>
    </>
  )
}

export default Button
