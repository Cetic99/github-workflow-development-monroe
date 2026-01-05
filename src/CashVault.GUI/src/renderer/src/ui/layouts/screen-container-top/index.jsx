/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import { keyframes } from '@emotion/react'
import LogoMedium from '@ui/components/icons/LogoMedium'
import ContactSupport from '@ui/components/contact-support'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@icons/IconRightHalfArrow'
import IconLeftHalfArrow from '@icons/IconLeftHalfArrow'
import IconClose from '@icons/IconClose'
import LockerInfo from '@ui/components/locker-info'
import { useLocation, useNavigate } from 'react-router-dom'
import IconFilter from '@ui/components/icons/IconFilter'
import { globalStore, useIsMultipleRoutersEnabled } from '@src/app/domain/global/stores'
import { useContext } from 'react'
import { HistoryContext } from '@ui/components/history-tracker'
import { useParcelStoreActions } from '@src/app/domain/parcel-locker/stores/parcel-store'
import IconUser from '@ui/components/icons/IconUser'
import IconBasicUser from '@ui/components/icons/IconBasicUser'
import IconLogout from '@ui/components/icons/IconLogout'

const fadeIn = keyframes`
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
`

const Wave = () => {
  return (
    <svg
      width="1024"
      height="106"
      viewBox="0 0 1024 106"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M336.091 55.7823C315.064 55.7823 295.41 45.129 281.522 28.9929L276.215 22.8161C263.734 8.31095 245.759 0 226.852 0H0V106H1024V55.7823H336.091Z"
        fill="var(--bg-medium)"
      />
    </svg>
  )
}

const Container = styled.div`
  width: 100dvw;
  height: 100dvh;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 1;

  animation: ${fadeIn} 0.5s ease-in;
`

const Header = styled.div`
  background-color: #004b3e;
  height: 7.75rem;
  width: 100%;
  position: relative;
  flex-shrink: 0;

  & .admin-button {
    position: absolute;
    top: 1rem;
    left: 1.5rem;
    cursor: pointer;
  }

  & .header-content {
    position: absolute;
    top: 0;
    right: 0;
    display: flex;
    flex-direction: row;
    gap: 0.25rem;
    height: 100%;
    align-items: center;
    width: calc(100% - 18rem);
    padding: 0 3.5rem 0 0;
  }

  & .exit-button {
    margin-left: auto;
  }
`

const Content = styled.div`
  flex-grow: 1;
  position: relative;
  /* Padding was moved to Main so the scrollbar is fully on the right,
   but if you want padding around the content inside the scroll, keep it here or in Main */

  padding: 7.5rem 0 0 0;
  height: calc(100dvh - 7.75rem);
  display: flex;
  flex-direction: column;

  & .wave {
    position: absolute;
    top: -3.5rem;
    left: 0;
    right: 0;
    width: 100%;
  }
`

const Main = styled.div`
  height: 100%;
  width: 100%;
  overflow-y: auto; /* This enables scrolling for the entire container */
  display: flex;
  flex-direction: column; /* Arranges the content and footer vertically */
  padding: 0 3.5rem; /* Padding moved here so the content doesn’t touch the edges */
`

/* Wrapper for children so flex works correctly with margin-top: auto on the footer */
const ChildrenWrapper = styled.div`
  flex: 1; /* Takes up all available space and pushes the footer down if there isn’t enough content */
  display: flex;
  flex-direction: column;
`

const AppLogo = styled.div`
  position: absolute;
  width: 100%;
  display: flex;
  gap: 2rem;
  padding: 0 3.5rem 0 3.5rem;
  top: 0;
  left: 0;
  z-index: 3;

  & .align-right {
    position: absolute;
    bottom: 0;
    right: 4.5rem;
  }
`

const Actions = styled.div`
  margin-top: auto;
  display: flex;
  gap: 4rem;
  padding-bottom: 2.5rem;
  padding-top: 1rem;
  justify-content: space-between;
  width: 100%;

  position: sticky;
  bottom: 0;
  z-index: 10;

  > div {
    min-width: 17rem;
  }
`

const ScreenContainerTop = ({
  children,
  actions = null,
  backgroundImage = () => <></>,
  supportVisible = true,
  infoVisible = true,
  hasAdminButton = true,
  hasExperienceSelector = true
}) => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const { setHistoryStack } = useContext(HistoryContext)
  const isMultipleRoutersEnabled = useIsMultipleRoutersEnabled()
  const { resetData } = useParcelStoreActions()

  const handleExit = () => {
    resetData()
    setHistoryStack([])

    globalStore.getState().actions.setSelectedRouter(null)

    navigate('/#', { replace: true })
  }

  const location = useLocation()

  const onAdminLogin = () => {
    const base = location.pathname.split('/')[1]

    if (base !== undefined && base !== null && base !== '')
      navigate(`/${base}/select-courier/operator-code`)

    navigate(`/parcel-locker/select-courier/operator-code`)
  }

  return (
    <Container>
      <Header>
        {hasAdminButton && (
          <div className="admin-button" onClick={() => onAdminLogin()}>
            <IconUser color="white" size="m" />
          </div>
        )}
        <div className="header-content">
          {infoVisible && <LockerInfo />}

          {isMultipleRoutersEnabled === true && hasExperienceSelector && (
            <CircleButton
              className="exit-button"
              size="s"
              color="inverted"
              icon={(props) => <IconLogout {...props} />}
              textLeft={t('Exit')}
              disabled={false}
              onClick={() => {
                resetData()
                handleExit()
              }}
            />
          )}
        </div>
      </Header>

      <Content>
        {backgroundImage()}

        <div className="wave">
          <Wave />
        </div>

        <AppLogo className="logo-container">
          <LogoMedium />

          {supportVisible && (
            <div className="align-right">
              <ContactSupport />
            </div>
          )}
        </AppLogo>

        <Main className="screen-container-content">
          <ChildrenWrapper>{children}</ChildrenWrapper>

          {actions && (
            <Actions>
              {actions?.onBack != null && (
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconLeftHalfArrow {...props} />}
                  textRight={t('Back')}
                  disabled={actions.onBackDisabled}
                  onClick={actions.onBack}
                  shadow={true}
                />
              )}
              {actions?.onCancel != null && (
                <CircleButton
                  size="l"
                  color="medium"
                  icon={(props) => <IconClose {...props} />}
                  textRight={t('Cancel')}
                  disabled={false}
                  onClick={actions.onCancel}
                  shadow={true}
                />
              )}

              {/* Empty div to preserve the layout when there are no left-side buttons */}
              {actions?.onBack == null && actions?.onCancel == null && <div />}

              <div style={{ display: 'flex', gap: '4rem' }}>
                {actions?.onFilter != null && (
                  <CircleButton
                    size="l"
                    //color="white"
                    icon={(props) => <IconFilter {...props} />}
                    textRight={t('Filter')}
                    disabled={false}
                    onClick={actions.onFilter}
                    shadow={true}
                  />
                )}
                {actions?.onLoginAdmin != null && (
                  <CircleButton
                    size="l"
                    textRight={t('Login as administrator')}
                    icon={(props) => <IconBasicUser {...props} />}
                    onClick={actions.onLoginAdmin}
                    shadow={true}
                  />
                )}
                {actions?.onProceed != null && (
                  <CircleButton
                    size="l"
                    icon={(props) => <IconRightHalfArrow {...props} />}
                    textRight={t('Proceed')}
                    disabled={actions.onProceedDisabled || false}
                    onClick={actions.onProceed}
                    shadow={true}
                  />
                )}
              </div>
            </Actions>
          )}
        </Main>
      </Content>
    </Container>
  )
}

export default ScreenContainerTop
