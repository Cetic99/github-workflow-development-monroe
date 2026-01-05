/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { useRef } from 'react'
import styled from '@emotion/styled'
import IconInfoCircle from '../icons/IconInfoCircle.jsx'
import IconWarning from '../icons/IconWarning.jsx'
import IconCheck from '../icons/IconCheck.jsx'
import { severityMap } from './severity.js'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'

const Container = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem;
  border-radius: 1rem;

  & .alert-body {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  & .alert-text {
    white-space: pre-wrap;
    word-break: break-word;

    display: -webkit-box;
    -webkit-line-clamp: ${(props) => props.maxRows || 3};
    -webkit-box-orient: vertical;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  background-color: ${(props) =>
    props.severity === severityMap.WARNING
      ? 'var(--warning-bg)'
      : props.severity === severityMap.ERROR
        ? 'var(--error-bg)'
        : props.severity === severityMap.INFO
          ? 'var(--info-bg)'
          : 'var(--bg-dark)'};

  color: ${(props) =>
    props.severity === severityMap.WARNING
      ? 'var(--secondary-dark)'
      : props.severity === severityMap.ERROR
        ? 'var(--error-dark)'
        : props.severity === severityMap.INFO
          ? 'var(--info-text)'
          : 'var(--text)'};
`

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
    text-transform: capitalize;
  }

  white-space: pre-wrap;
  word-break: break-word;
`

const Alert = ({
  severity = severityMap.SUCCESS,
  text = 'info',
  iconSize = 's',
  maxRows = 3,
  showModal = true,
  onClick,
  buttonText
}) => {
  const infoModalRef = useRef()

  const handleOpenInfoModal = () => {
    if (!showModal || onClick) return

    if (infoModalRef.current) {
      infoModalRef.current.showModal()
    }
  }

  const RenderIcon = () => {
    switch (severity) {
      case severityMap.WARNING:
        return <IconWarning color="var(--secondary-dark)" size={iconSize} />
      case severityMap.ERROR:
        return <IconWarning color="var(--error-dark)" size={iconSize} />
      case severityMap.INFO:
        return <IconInfoCircle size={iconSize} />
      case severityMap.SUCCESS:
        return <IconCheck size={iconSize} />
      default:
        return <IconInfoCircle />
    }
  }
  return (
    <>
      <Container
        className="alert"
        severity={severity}
        maxRows={maxRows}
        onClick={() => handleOpenInfoModal()}
      >
        <div className="alert-body">
          {RenderIcon()}
          <p className="alert-text">{text}</p>
        </div>
        {onClick && <Button onClick={onClick}>{buttonText}</Button>}
      </Container>
      {showModal && (
        <Modal
          size="m"
          ref={infoModalRef}
          onClose={() => {
            infoModalRef?.current?.close()
          }}
        >
          <ModalContent>
            <div className="modal-header">{severity}</div>
            {text}
          </ModalContent>
        </Modal>
      )}
    </>
  )
}

export default Alert
