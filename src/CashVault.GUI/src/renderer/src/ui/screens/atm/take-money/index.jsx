import ScreenHeading from '@ui/components/screen-heading'
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import IconTakeMoney from '@ui/components/icons/IconTakeMoney'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: auto;
  justify-content: center;
  width: 70%;
  margin: 0 auto;

  h1 {
    font-size: 4.375rem;
    font-weight: 800;
    line-height: 4.375rem;
    margin: 1.5rem 0;
  }

  h2 {
    font-size: 1.625rem;
    color: var(--primary-light);
    font-weight: 400;
  }

  & .take-money {
    width: 100%;
    max-width: 400px;

    .money-bill {
      animation: slide-out 1s linear forwards;
    }
  }

  @keyframes slide-out {
    0% {
      transform: translateY(-40px);
    }
    100% {
      transform: translateY(0px);
    }
  }
`

const TakeMoneyScreen = () => {
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
        <ScreenHeading
          top={() => <IconTakeMoney />}
          middle="Please take your money"
          bottom={() => 'Thank you for banking with Monroe.'}
          bottomColor="var(--primary-dark)"
        />
      </Container>
    </ScreenContainer>
  )
}

export default TakeMoneyScreen
