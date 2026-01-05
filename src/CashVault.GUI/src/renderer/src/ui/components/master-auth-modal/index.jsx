/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useEffect, useRef, useState } from 'react'
import styled from '@emotion/styled'
import { getMasterPassword } from '@src/config'
import Notifications from '@src/app/services/notifications'
import { useTranslation } from '@domain/administration/stores'
import Button from '@ui/components/button'
import IconShield from '@ui/components/icons/IconShield'
import TextInput from '@ui/components/inputs/text-input'
import Modal from '@ui/components/modal'

const Content = styled.div`
  padding: 1em 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
`

const ModalContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;

  & .header {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  & .action-buttons {
    display: flex;
    gap: 0.75rem;
  }
`

const MasterAuthModal = ({ open, onClose, onAuthenticated }) => {
  var [password, setPassword] = useState('admin')
  const modalRef = useRef(null)
  const { t } = useTranslation()

  useEffect(() => {
    if (open === true) {
      modalRef.current.showModal()
    }
  }, [open])

  const onConfirm = async () => {
    const masterPassword = await getMasterPassword()
    if (password === masterPassword) {
      onAuthenticated()
      modalRef.current?.close()
    } else {
      Notifications.error('Invalid password')
    }
  }

  const handleClose = () => {
    modalRef.current?.close()
    onClose()
  }

  return (
    <Modal ref={modalRef} onClose={handleClose}>
      <ModalContainer>
        <div className="header">
          <IconShield />
          <h1>Master authentication</h1>
        </div>

        <Content>
          <TextInput
            type="password"
            label={t('Password')}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </Content>

        <div className="action-buttons">
          <Button onClick={handleClose}>{t('Cancel')}</Button>

          <Button onClick={onConfirm}>{t('Confirm')}</Button>
        </div>
      </ModalContainer>
    </Modal>
  )
}

export default MasterAuthModal
