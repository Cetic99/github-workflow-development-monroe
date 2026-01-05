/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useEffect, useRef } from 'react'
import styled from '@emotion/styled'
import {
  useLocalizationActions,
  useCurrentLanguage,
  useTranslation
} from '@domain/administration/stores'
import { Language } from '@domain/administration/constants'
import Modal from '@ui/components/modal'
import Button from '@ui/components/button'

const Content = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  min-width: 200px;

  & .selector {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  & .actions {
    display: flex;
    flex-direction: row-reverse;
  }
`

const LanguageModal = ({ open, onClose }) => {
  const language = useCurrentLanguage()
  const { setCurrentLanguage } = useLocalizationActions()
  const modalRef = useRef(null)
  const { t } = useTranslation()

  useEffect(() => {
    if (open === true) {
      modalRef.current.showModal()
    }
  }, [open])

  const changeLanguage = (lang) => {
    if (language === lang) return

    setCurrentLanguage(lang)
    onClose()
  }

  const handleClose = () => {
    modalRef.current?.close()
    onClose()
  }

  return (
    <Modal ref={modalRef} onClose={handleClose}>
      <h1>{t('Language')}</h1>

      <Content>
        <div className="selector">
          <Button
            color={language === 'en' ? 'light' : 'dark'}
            onClick={() => changeLanguage(Language.EN)}
          >
            {t('ENGLISH')}
          </Button>

          <Button
            color={language === 'bs' ? 'light' : 'dark'}
            onClick={() => changeLanguage(Language.BS)}
          >
            {t('SERBIAN')}
          </Button>
        </div>

        <div className="actions">
          <Button onClick={handleClose}>{t('Cancel')}</Button>
        </div>
      </Content>
    </Modal>
  )
}

export default LanguageModal
