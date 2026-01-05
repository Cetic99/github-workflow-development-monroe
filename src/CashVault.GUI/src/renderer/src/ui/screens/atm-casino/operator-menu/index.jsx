/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import CardButton from '@ui/components/card-button'

import IconReceipt from '@ui/components/icons/IconReceipt'
import IconMoneyOut from '@ui/components/icons/IconMoneyOut'
import IconWarningTriangle from '@ui/components/icons/IconWarningTriangle'
import IconApp from '@ui/components/icons/IconApp'
import IconCashStack from '@ui/components/icons/IconCashStack'
import IconAuth from '@ui/components/icons/IconAuth'
import IconFileList from '@ui/components/icons/IconFileList'
import IconSettingsAdjust from '@ui/components/icons/IconSettingsAdjust'

import { useNavigate } from 'react-router-dom'
import Authorized from '@ui/components/authorized'
import { Permission } from '@domain/global/constants'
import { useAppVersion, useOperatorName } from '@src/app/domain/global/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-top: 8rem;
  margin-bottom: 2rem;

  .app-version {
    color: black;
    font-size: 1rem;
    text-align: right;
    position: absolute;
    top: 7.5rem;
    right: 4rem;
    font-weight: 400;
  }

  .user {
    color: black;
    font-weight: 700;
    font-size: 2.5rem;
    line-height: 2.5rem;
    text-align: right;
    margin-left: auto;
    margin-top: -8rem;

    .welcome {
      color: var(--secondary-dark);
      font-size: 1rem;
      line-height: 1rem;
      letter-spacing: 4%;
      text-transform: uppercase;
    }
  }

  .title {
    font-size: 1.75rem;
    font-weight: 600;
    margin-top: 4rem;
  }

  .widgets {
    display: flex;
    width: 100%;
    gap: 1rem;
    flex-wrap: wrap;

    & > div {
      flex-grow: 10;
    }
  }
`

const OperatorMenuScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const operatorName = useOperatorName()
  const appVersion = useAppVersion()

  return (
    <ScreenContainer
      isAdmin={true}
      overflow={false}
      padding={false}
      hasLogo={true}
      hasExitButton={true}
      hasOnGoBack={false}
      hasFadeLogo={true}
    >
      <Container>
        <Authorized
          permissions={[
            Permission.MoneyService,
            Permission.Reports,
            Permission.Logs,
            Permission.Administration,
            Permission.Maintenance
          ]}
        >
          <div className="app-version">
            {t('Version')}: {appVersion}
          </div>
          {operatorName && (
            <div className="user">
              <div className="welcome">{t('welcome')}</div>
              <div>{operatorName}</div>
            </div>
          )}
          <div className="title">{t('Please choose')}</div>
        </Authorized>
        <div className="widgets">
          <Authorized permissions={[Permission.MoneyService]}>
            <CardButton
              icon={<IconCashStack color={'var(--primary-light)'} size="l" />}
              text={t('Money service')}
              onClick={() => navigate('money-service')}
            />
          </Authorized>
          <Authorized permissions={[Permission.Reports]}>
            <CardButton
              icon={<IconReceipt color={'var(--primary-light)'} size="l" />}
              text={t('Reports')}
              onClick={() => navigate('reports')}
            />
          </Authorized>
          <Authorized permissions={[Permission.Logs]}>
            <CardButton
              icon={<IconFileList color={'var(--primary-light)'} size="l" />}
              text={t('Logs')}
              onClick={() => navigate('logs')}
            />
          </Authorized>
        </div>
        <div className="widgets">
          <Authorized permissions={[Permission.Administration]}>
            <CardButton
              icon={<IconAuth color={'var(--primary-light)'} size="l" />}
              text={t('Admin')}
              onClick={() => navigate('administration')}
            />
          </Authorized>
          <Authorized permissions={[Permission.Maintenance]}>
            <CardButton
              icon={<IconWarningTriangle color={'var(--primary-light)'} size="l" />}
              text={t('Maintenance')}
              onClick={() => navigate('maintenance')}
            />
          </Authorized>
        </div>
        <Authorized permissions={[Permission.Configuration]}>
          <div className="title">{t('Configuration')}</div>
          <div className="widgets">
            <CardButton
              icon={<IconMoneyOut color={'var(--primary-light)'} size="l" />}
              text={t('Money')}
              onClick={() => navigate('configuration-money')}
            />
            <CardButton
              icon={<IconSettingsAdjust color={'var(--primary-light)'} size="l" />}
              text={t('Customization')}
              onClick={() => navigate('configuration-customization')}
            />
            <CardButton
              icon={<IconApp color={'var(--primary-light)'} size="l" />}
              text={t('Device')}
              onClick={() => navigate('configuration-devices')}
            />
          </div>
        </Authorized>
      </Container>
    </ScreenContainer>
  )
}

export default OperatorMenuScreen
