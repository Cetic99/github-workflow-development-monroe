import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import Menu from '@ui/components/menu'
import { useAppNavigate } from '@domain/global/hooks/use-app-navigate'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: auto;
  h1 {
    font-size: 3.125rem;
    font-weight: 700;
    margin-bottom: 3rem;
  }
`

const SettingsScreen = () => {
  const navigate = useAppNavigate()

  const OPTIONS = [
    {
      title: 'Dark mode',
      value: true,
      toggle: true,
      onChange: (value) => {
        console.log(value)
      }
    },
    {
      title: 'Change location',
      value: 'Banja Luka',
      toggle: false,
      options: [
        { title: 'Banja Luka', value: 'Banja Luka' },
        { title: 'Mostar', value: 'Mostar' },
        { title: 'Beograd', value: 'Beograd' }
      ],
      onChange: (value) => {
        console.log(value)
      }
    },
    {
      title: 'Font size',
      value: 'Normal',
      toggle: false,
      options: [
        { title: 'Normal', value: 'Normal' },
        { title: 'Small', value: 'Small' }
      ],
      onChange: (value) => {
        console.log(value)
      }
    },
    {
      title: 'Change background',
      value: 'Video Bubbles',
      options: [
        { title: 'Video Bubbles', value: 'Video Bubbles' },
        { title: 'Static bubble', value: 'Static bubble' }
      ],
      toggle: false,
      onChange: (value) => {
        console.log(value)
      }
    },
    {
      title: 'Help & support',
      toggle: false,
      onChange: () => {}
    },
    {
      title: 'About',
      toggle: false,
      onChange: () => {}
    }
  ]

  return (
    <ScreenContainer
      hasLogo={true}
      hasOnGoBack={true}
      hasExitButton={false}
      hasMasterAuthButton={false}
      hasUserButton={false}
      hasLanguageSwitcher={false}
      hasSettingsButton={false}
      isBottomWave={true}
    >
      <Container>
        <h1>Settings</h1>
        <Menu options={OPTIONS} />
      </Container>
    </ScreenContainer>
  )
}

export default SettingsScreen
