/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import IconSettings from '@ui/components/icons/IconSettings'
import styled from '@emotion/styled'
import TextInput from '@ui/components/inputs/text-input'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import InvalidInputAlert from '@ui/components/invalid-input-alert'
import { useLoading } from '@domain/global/stores'
import { useTranslation } from '@domain/administration/stores'
import { Fragment, useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import FullPageLoader from '@ui/components/full-page-loader'
import { useGlobalActions } from '@domain/global/stores'
import { useAuthenticate } from '@domain/global/commands'
import { getAuthFailedStatus } from '@domain/global/stores'
import { setAuthFailedStatus } from '@domain/global/stores'
import NfcCard from '@ui/assets/images/nfc_admin_login.png'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/operator/commands'
import IconKeyboard from '@ui/components/icons/IconKeyboard'
import CircularTimer from '@ui/components/circle-timer'
import { useIsCardReaderReady } from '@src/app/domain/device/stores'

const Container = styled.div`
  margin-top: 8rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding-right: 4rem;
  height: 80%;

  & .timer {
    position: absolute;
    top: 12rem;
    right: 6rem;
  }

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

    & .login-with-username-button {
      text-transform: none;
      font-size: 1.75rem;
      font-weight: 500;
    }
  }

  .align-center {
    align-self: center;
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

const CardContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  flex-direction: column;
  justify-content: space-between;
  height: 100%;
  align-items: center;
  justify-content: center;

  & .content {
    display: flex;
    flex-direction: column;
    align-items: center;

    gap: 2rem;

    & .nfc-image {
      width: 24rem;
      height: 24rem;
      object-fit: contain;
      padding-top: 4rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 4rem;

      & .caption {
        width: 32rem;
        text-align: center;
        font-size: 1.625rem;
        color: black;
      }
    }
  }
`

const AdminLoginScreen = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const isLoading = useLoading()
  const isCardReaderReady = useIsCardReaderReady()

  const loginCommand = useAuthenticate()

  const { setLoading } = useGlobalActions()

  var [username, setUsername] = useState('')
  var [password, setPassword] = useState('')

  const [isNfcloading, setIsNfcLoading] = useState(true)

  const timeout = 10000
  const timerSeconds = 10
  const timeoutRef = useRef()

  useEffect(() => {
    Mediator.dispatch(CommandType.EnableUserLogin)

    let timerInterval = null

    timeoutRef.current = setTimeout(() => {
      setIsNfcLoading(false)
    }, timeout)

    return () => {
      if (timerInterval) clearInterval(timerInterval)
      Mediator.dispatch(CommandType.DisableUserLogin)
    }
  }, [])

  const usernameRef = useRef()
  const checkAuthFields = () => {
    if (username?.trim()?.length === 0 || password?.trim()?.length === 0) return true

    return false
  }

  const authfailedStatus = getAuthFailedStatus()

  const handleCancel = () => {
    setAuthFailedStatus(undefined)
    setPassword('')
    setUsername('')
    navigate('/')
  }

  const handleConfirm = () => {
    if (checkAuthFields()) return

    setLoading(true)

    loginCommand.mutate({ username, password })
  }

  const handleEnterClick = (e) => {
    if (e.key !== 'Enter') {
      return
    }

    handleConfirm()
  }

  const handleCancelNfc = () => {
    setIsNfcLoading(false)
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }
  }

  useEffect(() => {
    if (!isNfcloading || !isCardReaderReady) {
      usernameRef.current?.focus()
    }
  }, [isNfcloading])

  return (
    <ScreenContainer
      onGoBack={() => handleCancel()}
      hasUserButton={false}
      hasExperienceSelector={false}
    >
      <FullPageLoader loading={isLoading} />

      <Container>
        {isNfcloading && isCardReaderReady === true && (
          <>
            <CircularTimer className="timer" duration={timerSeconds} />
            <CardContainer>
              <div className="content">
                <div className="nfc-image">
                  <img src={NfcCard} alt="NFC Card" />
                  <p className="caption">
                    Please tap your NFC card to the reader, you will be logged in automatically.
                  </p>
                </div>
              </div>
            </CardContainer>
            <div className="wms-footer align-center">
              <CircleButton
                size="m"
                className="login-with-username-button"
                textRight={t('Login with username & password')}
                icon={(props) => <IconKeyboard {...props} />}
                onClick={() => handleCancelNfc()}
              />
            </div>
          </>
        )}

        {(!isNfcloading || !isCardReaderReady) && (
          <Fragment>
            <IconSettings size="xl" />
            <div className="header-text">{t('Please login.')}</div>
            <div className="form-input">
              <div className="label">{`${t('Username')}:`}</div>
              <TextInput
                ref={usernameRef}
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                onKeyDown={handleEnterClick}
              />
            </div>

            <div className="form-input">
              <div className="label">{`${t('Password')}:`}</div>
              <TextInput
                isPassword={true}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onKeyDown={handleEnterClick}
              />
              {authfailedStatus === false && (
                <InvalidInputAlert
                  message={t(
                    'Password or username is not valid. Please contact support if you have problems'
                  )}
                />
              )}
            </div>

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
          </Fragment>
        )}
      </Container>
    </ScreenContainer>
  )
}

export default AdminLoginScreen
