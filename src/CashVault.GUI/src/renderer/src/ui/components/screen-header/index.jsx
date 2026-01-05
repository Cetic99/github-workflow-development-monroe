/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconUser from '@ui/components/icons/IconUser'
import TimeAndDate from '@ui/components/time-and-date'
import { useContext } from 'react'
import { HistoryContext } from '@ui/components/history-tracker'
import useNetworkStatus from '@domain/global/hooks/use-network-status'
import { globalStore, useCmsStatus, useIsMultipleRoutersEnabled } from '@domain/global/stores'
import { useIsAuthenticated } from '@domain/global/stores'
import LanguageSwitcherButton from '@ui/components/language-switcher-button'
import IconClose from '@ui/components/icons/IconClose'
import IconAuth from '@ui/components/icons/IconAuth'
import IconWifiOn from '@ui/components/icons/IconWifiOn'
import IconWifiOff from '@ui/components/icons/IconWifiOff'
import { useTranslation } from '@domain/administration/stores'
import { useAppNavigate } from '@domain/global/hooks/use-app-navigate'
import { useNavigate } from 'react-router-dom'
import IconSettings from '@ui/components/icons/IconSettings'
import IconLogout from '../icons/IconLogout'

const Container = styled.main`
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
  position: relative;
  height: 6.5rem;

  .left-side {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 1.5rem;
    margin-top: -3rem;
    padding-left: 2rem;
  }

  .right-side {
    display: flex;
    flex-direction: row-reverse;
    align-items: center;
    gap: 2rem;
    padding: 1.5rem 2rem;
  }

  & .separator {
    width: 0.094rem;
    height: 2.875rem;
    background-color: var(--primary-medium);
  }
`

const ScreenHeader = ({
  hasOnGoBack = true,
  onGoBackProvided = () => {},
  hasLanguageSwitcher = true,
  hasExitButton = false,
  hasMasterAuthButton = true,
  hasUserButton = true,
  hasSettingsButton = false,
  hasTimeDate = true,
  urlPrefix,
  hasExperienceSelector = true
}) => {
  const { t } = useTranslation()
  const { canGoBack, onGoBack, setHistoryStack } = useContext(HistoryContext)
  const navigate = useAppNavigate()
  const nativeNavigate = useNavigate()
  const cmsStatus = useCmsStatus()
  const { isOnline } = useNetworkStatus()
  const isAuthenticated = useIsAuthenticated()
  const isMultipleRoutersEnabled = useIsMultipleRoutersEnabled()

  const onAdminLogin = () => {
    if (isAuthenticated) return
    navigate('/admin-login', 'up')
  }

  const handleLanguageSwitcherClick = () => {
    nativeNavigate('/language')
  }

  const handleLogoutClick = () => {
    navigate('/logout', 'down')
  }

  const handleMasterAuthClick = () => {
    nativeNavigate('master-auth')
  }

  const handleSettingsClick = () => {
    navigate(urlPrefix ? `/${urlPrefix}/settings` : '/settings', 'down')
  }

  const handleExperienceSelect = () => {
    nativeNavigate('/')

    setHistoryStack(['/'])

    globalStore.getState().actions.setSelectedRouter(null)
  }

  const renderConnectivityStatusIcon = () => {
    if (
      isOnline &&
      cmsStatus?.isCasinoManagementSystem === true &&
      cmsStatus?.isConnected === true
    ) {
      return <IconWifiOn color="green" size="m" />
    }

    if (isOnline) {
      return <IconWifiOn color="white" size="m" />
    }

    return <IconWifiOff color="red" size="m" />
  }

  return (
    <Container>
      <div className="left-side">
        {hasUserButton && (
          <>
            {isAuthenticated === false && (
              <div onClick={() => onAdminLogin()}>
                <IconUser color="white" size="m" />
              </div>
            )}
          </>
        )}
        <div>{renderConnectivityStatusIcon()}</div>
      </div>
      <div className="right-side">
        {canGoBack && hasOnGoBack === true && (
          <>
            <CircleButton
              color="light"
              size="s"
              icon={(props) => <IconLeftHalfArrow {...props} />}
              textLeft={t('Back')}
              onClick={() => {
                onGoBackProvided()
                onGoBack()
              }}
            />
            <div className="separator" />
          </>
        )}

        {hasExitButton && (
          <>
            <CircleButton
              color="white"
              textLeft={t('Exit')}
              icon={(props) => <IconClose {...props} />}
              onClick={handleLogoutClick}
            />
            <div className="separator" />
          </>
        )}
        {isMultipleRoutersEnabled &&
          !hasExitButton &&
          isAuthenticated === false &&
          hasExperienceSelector && (
            <>
              <CircleButton
                color="white"
                textLeft={t('Exit')}
                icon={(props) => <IconLogout {...props} />}
                onClick={handleExperienceSelect}
              />
              <div className="separator" />
            </>
          )}

        {hasSettingsButton && (
          <>
            <CircleButton
              color="white"
              icon={(props) => <IconSettings {...props} />}
              onClick={handleSettingsClick}
            />
            <div className="separator" />
          </>
        )}

        {hasLanguageSwitcher && (
          <>
            <LanguageSwitcherButton onClick={handleLanguageSwitcherClick} />
            <div className="separator" />
          </>
        )}
        {hasMasterAuthButton && hasExitButton && (
          <>
            <CircleButton
              color="medium"
              icon={(props) => <IconAuth {...props} />}
              onClick={handleMasterAuthClick}
            />
            <div className="separator" />
          </>
        )}
        {hasTimeDate && <TimeAndDate />}
      </div>
    </Container>
  )
}

export default ScreenHeader
