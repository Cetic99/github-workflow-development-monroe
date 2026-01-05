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
    top: 1rem;
    padding: 0;
    left: 50%;
    transform: translateX(-50%);
  }

  & .top-wave {
    position: absolute;
    top: 0.6rem;
    left: 50%;
    transform: translateX(-50%);
  }

  & .title {
    text-align: center;
    font-weight: 700;
    font-size: 4rem;
    line-height: 4.5rem;
    color: var(--text-dark);
    margin-bottom: 2rem;
  }

  & .modal-content {
    padding: 4.188rem 2.5rem 2.5rem 2.5rem;
    background-color: var(--bg-medium);
    width: 100%;
    border-radius: 2rem;
  }
`

const TopWave = () => {
  return (
    <svg
      className="top-wave"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="231.23116883116882 0 376.0207792207792 62.64"
      width="376.0207792207792"
      height="62.64"
    >
      <path
        fill="#E6ECE4"
        d="M456 0C477.676 8.42866e-07 496.667 11.4944 507.211 28.7205C516.239 43.4707 529.717 58.1396 547.011 58.1396H800C817.673 58.1396 832 72.4665 832 90.1396V1048C832 1065.67 817.673 1080 800 1080H32C14.3269 1080 0 1065.67 0 1048V90.1397C0 72.4666 14.3269 58.1396 32 58.1396H284.989C302.283 58.1396 315.761 43.4707 324.789 28.7206C335.333 11.4945 354.324 8.42867e-07 376 0H456Z"
      />
    </svg>
  )
}

const RetailModal = forwardRef(
  ({ open, children, title, onClose, hasClose = true, size = 'l' }, ref) => {
    const { t } = useTranslation()

    return (
      <Dialog open={open} ref={ref} size={size}>
        <TopWave />

        {hasClose && (
          <CircleButton
            className="close-button"
            color="transparent"
            onClick={onClose}
            size="s"
            icon={(props) => <IconClose {...props} />}
          />
        )}

        <div className="modal-content">
          {title && <div className="title">{title}</div>}
          {children}
        </div>
      </Dialog>
    )
  }
)

export default RetailModal
