/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { forwardRef } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import { useTranslation } from '@domain/administration/stores'

const Dialog = styled.dialog`
  transition: all 0.25s ease;
  width: 85%;
  margin: auto;
  overflow: auto;
  border: none;
  outline: none;
  position: relative;
  z-index: 1000;
  padding-top: 4.2rem;
  background-color: transparent;

  ${(p) => {
    if (p.size === 's') return 'width: 40%;'
    if (p.size === 'm') return 'width: 60%;'
    if (p.size === 'l') return 'width: 85%;'
  }}

  &:focus {
    outline: none;
  }

  &::backdrop {
    background-color: black;
    opacity: 0.7;
    pointer-events: none;
  }

  & .close-button {
    position: absolute;
    top: 1.35rem;
    right: 3.5rem;

    & .text-left {
      color: black;
    }

    & svg {
      color: var(--bg-medium);
    }
  }

  & .title {
    font-weight: 600;
    font-size: 1.5rem;
    line-height: 2rem;
    color: var(--text-dark);
    margin-bottom: 1.5rem;
  }

  & .modal-content {
    padding: 4.188rem 2.5rem 2.5rem 2.5rem;
    background-color: var(--bg-medium);
    width: 100%;
  }

  & .wave-svg {
    position: absolute;
    top: 0;
    right: 0;
    height: 4.2rem;
    width: auto;
    min-width: 100%;
    padding: 0rem 1rem;
  }
`

const WaveHeader = () => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      className="wave-svg"
      viewBox="0 0 913.7052631578947 59.07"
      preserveAspectRatio="none"
    >
      <path
        fill="#E7ECE4"
        d="M631.909 60.6565C652.936 60.6565 672.59 49.0723 686.478 31.5263L691.785 24.8097C704.266 9.03716 722.241 0 741.149 0H918V537H0V60.6565H631.909Z"
      />
    </svg>
  )
}

const Modal = forwardRef(({ open, children, title, onClose, hasClose = true, size = 'l' }, ref) => {
  const { t } = useTranslation()

  return (
    <Dialog open={open} ref={ref} size={size}>
      <WaveHeader />

      {hasClose && (
        <CircleButton
          className="close-button"
          textLeft={t('Close')}
          onClick={onClose}
          size="s"
          //color="inverted"
          icon={(props) => <IconClose {...props} />}
        />
      )}

      <div className="modal-content">
        {title && <div className="title">{title}</div>}
        {children}
      </div>
    </Dialog>
  )
})

export default Modal
