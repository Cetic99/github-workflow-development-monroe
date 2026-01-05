import { useNavigate } from 'react-router-dom'
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import NumberInput from '@ui/components/inputs/number-input'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
    margin-bottom: 4rem;
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

const ChooseAmountScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const handleInputChange = (value) => {
    //TODO: Implement this properly
    console.log(value)
  }

  const onGoBack = () => {
    navigate(-1)
  }

  const handleSubmit = () => {
    //TODO: Implement this function
    navigate('/atm/select-receipt?title=money+withdrawal')
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
          <IconBanknote01 size="xl" color="var(--secondary-dark)" />

          <h3>Money withdrawal</h3>
          <h1>
            Please enter
            <br /> amount
          </h1>
          <h2>Please enter the desired amount for withdrawal.</h2>
        </div>
        <NumberInput onChange={handleInputChange} />

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
            onClick={handleSubmit}
            shadow={true}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ChooseAmountScreen
