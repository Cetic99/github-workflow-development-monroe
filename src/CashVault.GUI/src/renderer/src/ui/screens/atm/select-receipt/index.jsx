import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate, useLocation } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import CardButton from '@ui/components/card-button'
import IconCheck from '@ui/components/icons/IconCheck'
import IconCircleRemove from '@ui/components/icons/IconCircleRemove'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding-top: 5rem;
  width: 90%;
  height: 100%;
  margin: 0 auto;

  & .header {
    margin-bottom: 3rem;
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

  & .cards {
    display: flex;
    gap: 1.25rem;
    width: 100%;

    & > * {
      width: 17.188rem;
    }
  }
`

const SelectReceiptScreen = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const queryParams = new URLSearchParams(location.search)
  const title = queryParams.get('title')
  const { t } = useTranslation()

  const onGoBack = () => {
    navigate(-1)
  }

  const handleReceiptPrintValue = (value) => {
    if (title.includes('deposit')) {
      navigate('/atm/loading?to=transaction-complete?success=true')
    } else {
      navigate('/atm/loading?to=take-card/money')
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
          <IconBanknote01 size="xl" color="var(--secondary-dark)" />

          <h3>{title}</h3>
          <h1>
            Would you like
            <br /> a receipt?
          </h1>
          <h2>Please select.</h2>
        </div>

        <div className="cards">
          <CardButton
            className="last-withdrawal"
            text={t('Yes')}
            icon={<IconCheck size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleReceiptPrintValue(true)}
          />
          <CardButton
            className="last-withdrawal"
            text={t('No')}
            icon={<IconCircleRemove size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleReceiptPrintValue(false)}
          />
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
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default SelectReceiptScreen
