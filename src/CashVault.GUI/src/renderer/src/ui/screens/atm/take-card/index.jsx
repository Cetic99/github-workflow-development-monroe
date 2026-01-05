import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import ScreenHeading from '@ui/components/screen-heading'
import { useNavigate, useParams } from 'react-router-dom'
import { useEffect } from 'react'
import IconTakeCard from '@ui/components/icons/IconTakeCard'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4.5rem 0;
  padding-top: 10.25rem;
  width: 70%;
  margin: 0 auto;
`

const TakeCardScreen = () => {
  const params = useParams()
  const navigate = useNavigate()
  const { money } = params

  useEffect(() => {
    if (money) {
      const timeout = setTimeout(() => {
        navigate('/atm/take-money')
      }, 5000)

      return () => clearTimeout(timeout)
    }
  }, [money, navigate])

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
          top={() => <IconTakeCard />}
          middle="Please take your card"
          bottom={() =>
            money ? 'Your money will follow after the card.' : 'Please take your card'
          }
          bottomColor="var(--primary-dark)"
        />
      </Container>
    </ScreenContainer>
  )
}

export default TakeCardScreen
