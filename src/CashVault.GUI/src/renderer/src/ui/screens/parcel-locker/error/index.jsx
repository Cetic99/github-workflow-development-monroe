/* eslint-disable prettier/prettier */
import { useLocation, useNavigate } from 'react-router-dom'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import IconWarning from '@ui/components/icons/IconWarning'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  overflow: auto;
  justify-content: space-between;
  height: 100%;

  h1 {
    font-size: 4.375rem;
    font-weight: 800;
    line-height: 4.375rem;
    margin: 1.5rem 0;
    color: var(--error-dark);
  }

  h2 {
    font-size: 1.625rem;
    color: white;
    font-weight: 400;
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

  & .content {
    margin-top: 3.125rem;

    h2 {
      font-size: 3.438rem;
      font-weight: 800;
      line-height: 3.438rem;
      color: white;
    }
  }
`

const ErrorScreen = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { t } = useTranslation()

  const queryParams = new URLSearchParams(location.search)

  const heading = queryParams.get('heading')
  const description = queryParams.get('description')
  const subheading = queryParams.get('subheading')

  const btn1Text = queryParams.get('btn1Text')
  const btn1Path = queryParams.get('btn1Path')

  const btn2Text = queryParams.get('btn2Text')
  const btn2Path = queryParams.get('btn2Path')

  const onBtn1Click = () => navigate(btn1Path)
  const onBtn2Click = () => navigate(btn2Path)

  return (
    <ScreenContainer
      hasLogo={true}
      hasOnGoBack={false}
      hasExitButton={false}
      hasMasterAuthButton={false}
      hasUserButton={false}
      hasLanguageSwitcher={false}
      hasSettingsButton={false}
      hasTimeDate={false}
      isBottomWave={true}
      backgroundColor={'#222222'}
    >
      <Container>
        <IconWarning size="xl" color="var(--error-dark)" />
        <h1>{t(heading) || 'Transaction Failed'}</h1>
        <h2>{t(description)}</h2>

        {subheading && (
          <div className="content">
            <h2>{t(subheading)}</h2>
          </div>
        )}

        <div className="footer">
          {btn1Path && (
            <CircleButton
              icon={(props) => <IconClose {...props} />}
              size="l"
              color="white"
              textRight={t(btn1Text)}
              onClick={onBtn1Click}
              outlined={true}
            />
          )}

          {btn2Path && (
            <CircleButton
              icon={(props) => <IconRightHalfArrow {...props} />}
              size="l"
              color="white"
              textRight={t(btn2Text)}
              onClick={onBtn2Click}
              outlined={true}
            />
          )}
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ErrorScreen
