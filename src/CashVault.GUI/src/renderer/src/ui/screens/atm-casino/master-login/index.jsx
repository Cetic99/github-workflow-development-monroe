/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import TextInput from '@ui/components/inputs/text-input'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import IconAuth from '@ui/components/icons/IconAuth'
import { getMasterPassword } from '@src/config'
import Notifications from '@ui/utility/notifications'

const Container = styled.div`
  margin-top: 8rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding-right: 4rem;

  .header-text {
    font-size: 3.438rem;
    font-weight: 600;
    font-weight: 700;
    margin-bottom: 1rem;
    line-height: 3.75rem;
  }

  & .wms-footer {
    padding-top: 1rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
  }

  .form-input {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;

    .label {
      color: var(--secondary-dark);
      font-weight: 500;
      font-size: 1.5rem;
    }
  }
`

const MasterLoginScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  var [password, setPassword] = useState('')
  const passwordRef = useRef(null)

  const checkAuthFields = () => {
    if (password?.trim()?.length === 0) return true

    return false
  }

  const handleCancel = () => {
    setPassword('')
    navigate('/')
  }

  useEffect(() => {
    if (passwordRef && passwordRef.current) passwordRef.current.focus()
  }, [passwordRef])

  const handleConfirm = async () => {
    const masterPassword = await getMasterPassword()
    if (password === masterPassword) navigate('/safe-mode')
    else Notifications.error('Invalid password')
  }

  const handleEnterClick = (e) => {
    if (e.key !== 'Enter') {
      return
    }

    handleConfirm()
  }
  return (
    <ScreenContainer onGoBack={() => handleCancel()}>
      <Container>
        <IconAuth size="xl" color="black" />
        <div className="header-text">{t('Master authentication')}</div>

        <TextInput
          ref={passwordRef}
          isPassword={true}
          value={password}
          label={t('Enter master password')}
          onChange={(e) => setPassword(e.target.value)}
          onKeyDown={handleEnterClick}
        />
        <div className="wms-footer">
          <CircleButton
            icon={(props) => <IconClose {...props} />}
            size="l"
            color="medium"
            textRight={t('Cancel')}
            onClick={() => handleCancel()}
          />

          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            disabled={checkAuthFields()}
            onClick={() => handleConfirm()}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default MasterLoginScreen
