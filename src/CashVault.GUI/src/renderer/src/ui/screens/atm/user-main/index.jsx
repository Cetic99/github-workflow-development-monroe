import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import MoneyWithdrawal from '@ui/components/money-withdrawal'
import MoneyOptions from '@ui/components/money-options'
import IconReceipt from '@ui/components/icons/IconReceipt'
import IconLock from '@ui/components/icons/IconLock'
import IconMoneyIn from '@ui/components/icons/IconMoneyIn'
import IconLockOpen from '@ui/components/icons/IconLockOpen'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4.5rem 0;
  padding-top: 10.25rem;
`

const options = [
  {
    title: 'Check balance',
    route: '/atm/check-balance',
    icon: <IconReceipt size="l" color="var(--primary-light)" />
  },
  {
    title: 'Deposit money',
    route: '/atm/deposit',
    icon: <IconMoneyIn size="l" color="var(--primary-light)" />
  },
  {
    title: 'Change PIN',
    route: '/atm/change-pin',
    icon: <IconLock size="l" color="var(--primary-light)" />
  },
  {
    title: 'Unblock PIN',
    route: '/atm/unblock-pin',
    icon: <IconLockOpen size="l" color="var(--primary-light)" />
  }
]

const moneyAmountOptions = [300, 100, 30, 20, 50]

const UserMainScreen = () => {
  return (
    <ScreenContainer
      hasLogo={true}
      hasOnGoBack={false}
      hasExitButton={false}
      hasMasterAuthButton={false}
      hasUserButton={false}
      hasLanguageSwitcher={false}
      hasSettingsButton={true}
      hasFadeLogo={true}
      urlPrefix="atm"
    >
      <Container>
        <MoneyWithdrawal moneyOptions={moneyAmountOptions} lastWithdrawalAmount={500} />
        <MoneyOptions options={options} />
      </Container>
    </ScreenContainer>
  )
}

export default UserMainScreen
