/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useState, useEffect, useRef } from 'react'
import { useTranslation } from '@domain/administration/stores'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'
import TextInput from '@ui/components/inputs/text-input'

const Container = styled.div`
  &::-webkit-scrollbar {
    display: none;
  }

  display: flex;
  flex-direction: column;
  gap: 1rem;
`

const ModalContent = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;

  & .action-button {
    align-self: flex-end;
  }
`

const AppSettingsModal = ({ open, onClose, settings, onSave = () => {} }) => {
  const [appSettings, setAppSettings] = useState({})
  const modalRef = useRef(null)
  const { t } = useTranslation()

  useEffect(() => {
    setAppSettings(settings)
  }, [settings])

  useEffect(() => {
    if (open === true) {
      modalRef.current?.showModal()
    }
  }, [open])

  const handleOnSave = () => {
    onSave(appSettings)
    modalRef.current?.close()
  }

  const handleOnClose = () => {
    onClose()
    modalRef.current?.close()
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setAppSettings({ ...appSettings, [name]: value })
  }

  return (
    <Modal ref={modalRef} onClose={handleOnClose}>
      <ModalContent>
        <h1>{t('Edit settings')}</h1>

        <Container>
          {Object.entries(appSettings).map(([key, value]) => (
            <TextInput key={key} name={key} label={key} onChange={handleChange} value={value} />
          ))}
        </Container>

        <Button className="action-button" onClick={handleOnSave}>
          {t('Save')}
        </Button>
      </ModalContent>
    </Modal>
  )
}

export default AppSettingsModal
