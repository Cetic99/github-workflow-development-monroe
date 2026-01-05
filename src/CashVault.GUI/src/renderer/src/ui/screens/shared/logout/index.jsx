/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import IconGlobe from '@ui/components/icons/IconGlobe'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconClose from '@ui/components/icons/IconClose'
import { useContext } from 'react'
import { HistoryContext } from '@ui/components/history-tracker'
import { useNavigate } from 'react-router-dom'
import { useGlobalActions } from '@domain/global/stores'
import { useLogout } from '@domain/global/commands'
import { useAppNavigate } from '@domain/global/hooks/use-app-navigate'

const Container = styled.div`
  display: flex;
  flex-direction: column;

  .title {
    font-size: 3.438rem;
    line-height: 3.75rem;
    font-weight: 700;
    color: var(--text-light);
    max-width: 30rem;
    letter-spacing: -4%;
    margin-top: 1rem;
    min-height: 8rem;
  }

  .actions {
    display: flex;
    gap: 5rem;
    margin-top: 5rem;
    width: 100%;
    align-items: center;
  }
`

const LogoutScreen = ({ onGoBackProvided = () => {} }) => {
  const { t } = useTranslation()
  const { onGoBack } = useContext(HistoryContext)
  const nativeNavigate = useNavigate()
  const navigate = useAppNavigate()
  const { logout } = useGlobalActions()
  const logoutCommand = useLogout(() => {
    logout()
    nativeNavigate('/')
  })

  const onLogout = () => {
    logoutCommand.mutate()
  }

  return (
    <ScreenContainer hasLanguageSwitcher={false} isBottomWave={true} hasOnGoBack={false}>
      <Container>
        <IconGlobe size="xl" color="white" />
        <p className="title">{t('Are you sure you want to exit?')}</p>
        <div className="actions">
          <CircleButton
            icon={(props) => <IconLeftHalfArrow {...props} />}
            size="l"
            color="light"
            textRight={t('No')}
            onClick={() => {
              navigate(-1, 'up')
              onGoBackProvided()
              onGoBack()
            }}
          />
          <CircleButton
            icon={(props) => <IconClose {...props} />}
            size="l"
            color="white"
            textRight={t('Yes, exit')}
            onClick={onLogout}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default LogoutScreen
