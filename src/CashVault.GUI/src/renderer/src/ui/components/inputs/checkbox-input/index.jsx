/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'
import { useRef } from 'react'
import Modal from '@ui/components/modal'

const Container = styled.div`
  position: relative;
  width: fit-content;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;

  padding: ${(p) => (p.size == 's' ? '0.1875rem' : '0.25rem')};

  & .hidden-checkbox {
    margin: 0;
    padding: 0;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    opacity: 0;
    z-index: 3;
  }

  & .label {
    font-weight: 600;
    font-size: ${(p) => (p.size == 's' ? '1.25rem' : '1.625rem')};
    opacity: ${(p) => (p.disabled ? 0.6 : 1)};
  }
`

const Checkbox = styled.div`
  z-index: 2;
  display: flex;
  align-items: center;
  justify-content: center;

  width: ${(p) => (p.size == 's' ? '1.625rem' : '2.25rem')};
  height: ${(p) => (p.size == 's' ? '1.625rem' : '2.25rem')};
  border-radius: 50%;
  border: 2px solid ${(p) => (p.checked ? 'var(--primary-dark)' : 'black')};
  background-color: ${(p) => (p.checked ? 'var(--primary-dark)' : 'transparent')};
  color: white;

  & svg {
    width: ${(p) => (p.size == 's' ? '0.8125rem' : '1.125rem')};
    height: ${(p) => (p.size == 's' ? '0.5625rem' : '0.78125rem')};
  }
`

const Checkmark = ({ className }) => {
  return (
    <svg
      className={className}
      width="13"
      height="9"
      viewBox="0 0 13 9"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M11.0215 0.378723C10.6709 0.378723 10.3038 0.496981 10.0356 0.765105L4.93442 5.92133C4.58104 6.27608 4.22628 6.21006 3.94853 5.79344L2.57768 3.73094C2.15693 3.09845 1.27829 2.92247 0.64854 3.34322C0.0187912 3.76534 -0.157217 4.64535 0.262158 5.27647L1.63443 7.33896C2.87193 9.2007 5.28644 9.43717 6.86356 7.85593L12.0074 2.74233C12.5423 2.20471 12.5423 1.30273 12.0074 0.765105C11.7393 0.496981 11.3722 0.378723 11.0215 0.378723Z"
        fill="currentColor"
      />
    </svg>
  )
}

const CheckboxInput = ({
  className = '',
  value = true,
  label,
  info,
  onChange = () => {},
  size = 's',
  disabled = false
}) => {
  const infoModalRef = useRef()
  const inputRef = useRef()

  const handleOpenInfoModal = () => {
    if (infoModalRef.current) {
      infoModalRef.current.showModal()
    }
  }

  const handleChange = (e) => {
    if (disabled) return
    onChange(e)
  }
  return (
    <>
      <Container className={className} size={size} checked={value} disabled={disabled}>
        <input
          ref={inputRef}
          checked={value}
          className="hidden-checkbox"
          type="checkbox"
          onChange={handleChange}
        />
        <Checkbox size={size} checked={value}>
          {value && <Checkmark />}
        </Checkbox>
        {label && <div className="label">{label}</div>}
        {info && (
          <div className="info" onClick={() => handleOpenInfoModal()}>
            <IconInfoSquare color="black" />
          </div>
        )}
      </Container>
      <Modal
        ref={infoModalRef}
        onClose={() => {
          infoModalRef?.current?.close()
        }}
      >
        {info}
      </Modal>
    </>
  )
}

export default CheckboxInput
