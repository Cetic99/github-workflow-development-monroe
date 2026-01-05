import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainer from '@ui/components/screen-container'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import IconLock from '@ui/components/icons/IconLock'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import PinInput from '@ui/components/inputs/pin-input'
import { useState } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
    h3 {
      font-size: 1rem;
      text-transform: uppercase;
      color: var(--secondary-dark);
      margin-top: 1.5rem;
    }

    h1 {
      font-size: 4.375rem;
      font-weight: 800;
      line-height: 4.375rem;
      margin-top: 1rem;
    }

    h2 {
      font-size: 1.625rem;
      color: var(--primary-dark);
      font-weight: 400;
      margin-top: 1.5rem;
    }
  }

  & .footer {
    padding-top: 3rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;
    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const ChangePinScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const [step, setStep] = useState(1)
  const [newPin, setNewPin] = useState('')
  const [error, setError] = useState(null)

  const handleComplete = (pin) => {
    if (step === 1) {
      setNewPin(pin)
      setStep(2)
    } else if (step === 2) {
      if (pin === newPin) {
        // TODO: Kompletna implementacija
        navigate('/atm/changed-pin?success=true')
      } else {
        navigate('/atm/changed-pin?success=false')
      }
    }
  }

  const onGoBack = () => {
    if (step === 2) {
      setStep(1)
      setError(null)
    } else {
      navigate(-1)
    }
  }

  return (
    <ScreenContainer
      hasLogo={true}
      hasOnGoBack={false}
      hasExitButton={false}
      hasMasterAuthButton={false}
      hasUserButton={false}
      hasLanguageSwitcher={false}
      hasSettingsButton={true}
      urlPrefix="atm"
    >
      <Container>
        <div className="header">
          <IconLock size="xl" color="var(--secondary-dark)" />

          <h3>Change PIN</h3>
          {step === 1 ? (
            <>
              <h1>
                Please enter your
                <br /> new PIN
              </h1>
              <h2>Enter your new desired PIN using the ATM keypad.</h2>
            </>
          ) : (
            <>
              <h1>
                Repeat your new
                <br /> PIN to confirm
              </h1>
              <h2>Re-enter the same PIN for confirmation.</h2>
            </>
          )}
          {error && <h2 style={{ color: 'red' }}>{error}</h2>}
        </div>

        <div className="input">
          <PinInput length={4} onComplete={handleComplete} />
        </div>

        <div className="footer">
          <CircleButton
            icon={(props) => <IconLeftHalfArrow {...props} />}
            size="l"
            color="medium"
            textRight={t('Back')}
            onClick={onGoBack}
            shadow={true}
          />
          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            onClick={onGoBack}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ChangePinScreen
