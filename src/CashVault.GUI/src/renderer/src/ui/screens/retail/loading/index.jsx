import styled from '@emotion/styled'
import RetailContainer from '@ui/components/retail-container'
import { useLocation, useNavigate } from 'react-router-dom'
import { useEffect } from 'react'
import IconSettings from '@ui/components/icons/IconSettings'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: center;

  width: 90%;
  margin: 0 auto;

  margin-bottom: 1.5rem;
  gap: 0 1.5rem;

  height: 60vh;

  .cogwheel {
    width: fit-content;
    height: fit-content;
    animation: spin-loading 8s linear infinite;
  }
  @keyframes spin-loading {
    from {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(360deg);
    }
  }

  h1 {
    font-size: 4rem;
    line-height: 4.5rem;
    color: white;
    font-weight: 700;
    margin-bottom: 1.375rem;
  }
  p {
    font-size: 1.5rem;
    line-height: 2rem;
    font-weight: 400;
    color: var(--primary-light);
  }
`

const LoadingScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const location = useLocation()

  const queryParams = new URLSearchParams(location.search)
  const redirectTo = queryParams.get('to') || '/retail/'

  useEffect(() => {
    const timeout = setTimeout(() => {
      navigate('/retail/' + redirectTo)
    }, 5000)

    return () => clearTimeout(timeout)
  }, [navigate, redirectTo])

  return (
    <RetailContainer
      color="secondary"
      hasFaqButton={false}
      hasCartButton={false}
      hasHomeButton={false}
    >
      <Container>
        <div className="cogwheel">
          <IconSettings size="xl" color="var(--primary-light)" />
        </div>
        <h1>{t('One moment, please')}</h1>
        <p>Your payment is being processed...</p>
      </Container>
    </RetailContainer>
  )
}

export default LoadingScreen
