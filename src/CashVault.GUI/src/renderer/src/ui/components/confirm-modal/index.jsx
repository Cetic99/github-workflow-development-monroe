/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */

import { useEffect, useRef } from 'react'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'

const ModalContent = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;

  & .action-buttons {
    display: flex;
    gap: 1rem;
  }
`
const ConfirmModal = ({
  open,
  onClose,
  header,
  text,
  confirmLabel = 'Confirm',
  onConfirm = () => {}
}) => {
  const { t } = useTranslation()
  const modalRef = useRef(null)

  useEffect(() => {
    if (open === true) {
      modalRef.current.showModal()
    }
  }, [open])

  const handleClose = () => {
    modalRef.current?.close()
    onClose()
  }

  return (
    <Modal ref={modalRef} onClose={handleClose}>
      <ModalContent>
        <h1>{header}</h1>

        <p>{text}</p>

        <div className="action-buttons">
          <Button onClick={handleClose}>{t('Cancel')}</Button>

          <Button onClick={onConfirm}>{confirmLabel}</Button>
        </div>
      </ModalContent>
    </Modal>
  )
}

export default ConfirmModal
